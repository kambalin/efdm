﻿using EFDM.Abstractions.Audit;
using EFDM.Abstractions.DAL.Providers;
using EFDM.Abstractions.Models.Domain;
using EFDM.Core.Constants;
using EFDM.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EFDM.Core.Audit
{
    public class DBContextAuditor : IDBContextAuditor
    {
        #region fields & properties

        public bool Enabled { get; set; }
        public ConcurrentDictionary<string, byte> GlobalIgnoredProperties { get; } = new ConcurrentDictionary<string, byte>();
        public ConcurrentDictionary<Type, byte> IncludedTypes { get; } = new ConcurrentDictionary<Type, byte>();
        public ConcurrentDictionary<Type, HashSet<string>> IgnoredTypeProperties { get; } = new ConcurrentDictionary<Type, HashSet<string>>();
        public ConcurrentDictionary<Type, HashSet<string>> OnlyIncludedTypeProperties { get; } = new ConcurrentDictionary<Type, HashSet<string>>();

        protected ConcurrentDictionary<Type, IMappingInfo> Mappings { get; } = new ConcurrentDictionary<Type, IMappingInfo>();
        protected ConcurrentDictionary<Type, List<int>> ExcludedTypeStateActions { get; } = new ConcurrentDictionary<Type, List<int>>();
        protected Func<IAuditEvent, IEventEntry, object, Task> EventCommonAction { get; set; }
        protected readonly IAuditableDBContext Context;

        #endregion fields & properties

        #region constructors

        public DBContextAuditor(IAuditableDBContext context, IAuditSettings auditSettings)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));

            if (auditSettings != null)
            {
                Enabled = auditSettings.Enabled;
                if (auditSettings.ExcludedTypeStateActions != null)
                    ExcludedTypeStateActions = auditSettings.ExcludedTypeStateActions;
                if (auditSettings.GlobalIgnoredProperties != null)
                    GlobalIgnoredProperties = auditSettings.GlobalIgnoredProperties;
                if (auditSettings.IncludedTypes != null)
                    IncludedTypes = auditSettings.IncludedTypes;
                if (auditSettings.IgnoredTypeProperties != null)
                    IgnoredTypeProperties = auditSettings.IgnoredTypeProperties;
                if (auditSettings.OnlyIncludedTypeProperties != null)
                    OnlyIncludedTypeProperties = auditSettings.OnlyIncludedTypeProperties;
            }
        }

        #endregion constructors

        #region IDBContextAuditor implementation

        public async Task<int> SaveChangesAsync(Func<Task<int>> baseSaveChangesAsync,
            CancellationToken cancellationToken = default)
        {
            if (!Enabled)
                return await baseSaveChangesAsync();
            var auditEvent = await CreateAuditEvent();
            if (auditEvent == null)
                return await baseSaveChangesAsync();
            try
            {
                auditEvent.Result = await baseSaveChangesAsync();
            }
            catch (Exception ex)
            {
                auditEvent.Success = false;
                auditEvent.ErrorMessage = ex.ToString();
                throw;
            }
            auditEvent.Success = true;

            foreach (var entry in auditEvent.Entries)
            {
                var entityAuditEvent = Activator.CreateInstance(GetEventType(entry.EntityType));
                var mapperEventAction = GetMapperEventAction(entry.EntityType);
                await mapperEventAction(auditEvent, entry, entityAuditEvent);
            }

            return auditEvent.Result;
        }

        public Func<IAuditEvent, IEventEntry, object, Task> GetMapperEventAction(Type type)
        {
            return async (auditEvent, entry, auditObj) =>
            {
                Mappings.TryGetValue(type, out IMappingInfo map);
                await map?.EventAction?.Invoke(auditEvent, entry, auditObj);
                if (EventCommonAction != null)
                    await EventCommonAction(auditEvent, entry, auditObj);
                else
                    return;
            };
        }

        public Type GetEventType(Type type)
        {
            Mappings.TryGetValue(type, out IMappingInfo map);
            return map?.AuditEventType;
        }

        public Type GetPropertyType(Type type)
        {
            Mappings.TryGetValue(type, out IMappingInfo map);
            return map?.AuditPropertyType;
        }

        public void ExcludeProperty<T>(Expression<Func<T, object>> propertySelector)
        {
            MemberExpression memberExpression;
            if (propertySelector.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)propertySelector.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
                memberExpression = (MemberExpression)propertySelector.Body;
            var memberName = memberExpression.Member.Name;
            GlobalIgnoredProperties.AddOrUpdate(memberName, 1, (key, oldValue) => 1);
        }

        public void IncludeAuditEntity(Type entityType)
        {
            IncludedTypes.AddOrUpdate(entityType, 1, (key, oldValue) => 1);
        }

        public void Map<TSourceEntity, TAuditEventEntity, TAuditPropertyEntity>(
            Action<IAuditEvent, IEventEntry, TAuditEventEntity> eventAction)
        {

            Mappings[typeof(TSourceEntity)] = new MappingInfo()
            {
                AuditEventType = typeof(TAuditEventEntity),
                AuditPropertyType = typeof(TAuditPropertyEntity),
                EventAction = (auditEvent, entry, eventEntity) =>
                {
                    eventAction.Invoke(auditEvent, entry, (TAuditEventEntity)eventEntity);
                    return Task.FromResult(true);
                }
            };
        }

        public void ExcludeTypeStateActions<TSourceEntity>(List<int> actions)
        {
            ExcludedTypeStateActions[typeof(TSourceEntity)] = actions;
        }

        public void SetEventCommonAction<T>(Func<IAuditEvent, IEventEntry, T, Task> entityAction)
        {
            EventCommonAction = (auditEvent, entry, auditEntity) =>
            {
                return entityAction(auditEvent, entry, (T)auditEntity);
            };
        }

        #endregion IDBContextAuditor implementation

        protected async Task<IAuditEvent> CreateAuditEvent()
        {
            var modifiedEntries = GetModifiedEntries();
            if (modifiedEntries.Count == 0)
                return null;
            var efEvent = new AuditEvent()
            {
                Entries = new List<IEventEntry>(),
                ContextId = Context.DbContext.ContextId.ToString()
            };
            foreach (var entry in modifiedEntries)
            {
                var entityName = GetEntityName(entry);
                efEvent.Entries.Add(new EventEntry()
                {
                    Entry = entry,
                    EntityType = entry.Entity.GetType(),
                    Action = GetStateAction(entry.State),
                    Changes = entry.State == EntityState.Modified ? await GetChanges(entry) : null,
                    Table = entityName.Table,
                    Schema = entityName.Schema,
                    Name = entry.Metadata.DisplayName(),
                    ColumnValues = GetColumnValues(entry)
                });
            }
            return efEvent;
        }

        protected static string GetColumnName(IProperty prop)
        {
            var storeObjectIdentifier = StoreObjectIdentifier.Create(prop.DeclaringType, StoreObjectType.Table);
            return storeObjectIdentifier.HasValue
                ? prop.GetColumnName(storeObjectIdentifier.Value)
                : prop.GetDefaultColumnName();
        }

        protected Dictionary<string, object> GetColumnValues(EntityEntry entry)
        {
            var result = new Dictionary<string, object>();
            var props = entry.Metadata.GetProperties();
            foreach (var prop in props)
            {
                PropertyEntry propEntry = entry.Property(prop.Name);
                if (IncludeProperty(entry, prop.Name))
                {
                    object value = entry.State != EntityState.Deleted ?
                        propEntry.CurrentValue : propEntry.OriginalValue;
                    result.Add(GetColumnName(prop), value);
                }
            }
            return result;
        }

        protected async Task<List<IEventEntryChange>> GetChanges(EntityEntry entry)
        {
            var result = new List<IEventEntryChange>();
            var props = entry.Metadata.GetProperties();
            var entityType = entry.Entity.GetType();
            var navigations = Context.DbContext.Model.FindEntityType(entityType)
                .GetNavigations().ToList();
            foreach (var prop in props)
            {
                PropertyEntry propEntry = entry.Property(prop.Name);
                if (propEntry.IsModified)
                {
                    if (IncludeProperty(entry, prop.Name))
                        result.Add(await GetPropertyChanges(propEntry, navigations, prop));
                }
            }
            return result;
        }

        protected async Task<EventEntryChange> GetPropertyChanges(PropertyEntry propEntry,
            List<INavigation> navigations, IProperty prop)
        {
            var eec = new EventEntryChange()
            {
                ColumnName = GetColumnName(prop),
                NewValue = propEntry.CurrentValue,
                OriginalValue = propEntry.OriginalValue
            };
            if (navigations == null)
                return eec;
            var navProp = navigations
                .Where(x => x.ForeignKey.Properties.Any(
                    y => y.Name == propEntry.Metadata.PropertyInfo.Name)
                ).FirstOrDefault();
            if (navProp == null)
                return eec;
            var relatedType = navProp.ForeignKey.DependentToPrincipal.ClrType;
            var dbSet = Context.DbContext.Set(relatedType) as IQueryable<IEntity>;
            if (eec.OriginalValue != null)
            {
                var newRelated = await dbSet?.AsNoTracking().Where(x => x.Id.Equals(eec.NewValue))
                    .FirstOrDefaultAsync();
                if (newRelated != null)
                    eec.NewValue = GetLookupValue(newRelated);
            }
            if (eec.OriginalValue != null)
            {
                var oldRelated = await dbSet?.AsNoTracking().Where(x => x.Id.Equals(eec.OriginalValue))
                    .FirstOrDefaultAsync();
                if (oldRelated != null)
                    eec.OriginalValue = GetLookupValue(oldRelated);
            }
            return eec;
        }

        protected string GetLookupValue(object entity)
        {
            // TODO: <int>
            return $"{(entity as IIdKeyEntity<int>)?.Id}: {(entity as ITitleEntity)?.Title}";
        }

        protected bool IncludeProperty(EntityEntry entry, string propName)
        {
            var entityType = GetDefiningType(entry)?.ClrType;
            if (entityType == null)
                return true;
            var ignoredProperties = EnsurePropertiesIgnoreAttrCache(entityType);
            if (ignoredProperties != null && ignoredProperties.Contains(propName))
                return false;
            if (GlobalIgnoredProperties != null
                && GlobalIgnoredProperties.ContainsKey(propName))
            {
                return false;
            }
            var onlyIncludedProperties = EnsurePropertiesOnlyIncludedAttrCache(entityType);
            if (onlyIncludedProperties != null && !onlyIncludedProperties.Contains(propName))
                return false;
            return true;
        }

        protected HashSet<string> EnsurePropertiesIgnoreAttrCache(Type type)
        {
            if (!IgnoredTypeProperties.ContainsKey(type))
                IgnoredTypeProperties[type] = null;
            return IgnoredTypeProperties[type];
        }

        protected HashSet<string> EnsurePropertiesOnlyIncludedAttrCache(Type type)
        {
            if (!OnlyIncludedTypeProperties.ContainsKey(type))
                OnlyIncludedTypeProperties[type] = null;
            return OnlyIncludedTypeProperties[type];
        }

        protected EntityDBData GetEntityName(EntityEntry entry)
        {
            var result = new EntityDBData();
            var definingType = GetDefiningType(entry);
            if (definingType == null)
                return result;
            result.Table = definingType.GetTableName();
            result.Schema = definingType.GetSchema();
            return result;
        }

        protected IReadOnlyEntityType GetDefiningType(EntityEntry entry)
        {
            IReadOnlyEntityType definingType =
                entry.Metadata.FindOwnership()?.DeclaringEntityType ??
                Context.DbContext.Model.FindEntityType(entry.Metadata.Name);
            return definingType;
        }

        protected List<EntityEntry> GetModifiedEntries()
        {
            return Context.DbContext.ChangeTracker.Entries()
                .Where(x => x.State != EntityState.Unchanged
                         && x.State != EntityState.Detached
                         && IncludeEntity(x))
                .ToList();
        }

        protected bool IncludeEntity(EntityEntry entry)
        {
            var type = entry.Entity.GetType();
            if (type.FullName.StartsWith("Castle.Proxies."))
                type = type.GetTypeInfo().BaseType;
            if (IncludedTypes != null && !IncludedTypes.ContainsKey(type))
                return false;
            if (ExcludedTypeStateActions.ContainsKey(type))
            {
                var excludedActions = ExcludedTypeStateActions[type];
                if (excludedActions != null && excludedActions.Contains(GetStateAction(entry.State)))
                    return false;
            }
            return true;
        }

        protected int GetStateAction(EntityState state)
        {
            switch (state)
            {
                case EntityState.Added:
                    return AuditStateActionVals.Insert;
                case EntityState.Modified:
                    return AuditStateActionVals.Update;
                case EntityState.Deleted:
                    return AuditStateActionVals.Delete;
                default:
                    return AuditStateActionVals.Unknown;
            }
        }
    }
}
