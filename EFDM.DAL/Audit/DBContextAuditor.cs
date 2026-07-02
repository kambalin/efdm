using EFDM.Abstractions.Audit;
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
        private readonly List<(object Entity, object Parent)> _queuedAuditEntities = new List<(object, object)>();
        protected ConcurrentDictionary<Type, Func<object, string>> LookupValueCache { get; } = new ConcurrentDictionary<Type, Func<object, string>>();
        /// <summary>
        /// Optional global resolver. If set and returns non-null for an entity, its value is used.
        /// </summary>
        public Func<object, string> LookupValueResolver { get; set; }

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

        public Task<int> SaveChangesAsync(Func<Task<int>> baseSaveChangesAsync,
            CancellationToken cancellationToken = default)
            => SaveChangesCore(false, baseSaveChangesAsync, null, cancellationToken);

        public int SaveChanges(Func<int> baseSaveChanges)
            => SaveChangesCore(true, null, baseSaveChanges, CancellationToken.None).GetAwaiter().GetResult();

        private async Task<int> SaveChangesCore(bool sync, Func<Task<int>> baseSaveChangesAsync,
            Func<int> baseSaveChanges, CancellationToken cancellationToken)
        {
            if (!Enabled)
                return sync ? baseSaveChanges() : await baseSaveChangesAsync().ConfigureAwait(false);
            var auditEvent = sync ? CreateAuditEvent() : await CreateAuditEventAsync().ConfigureAwait(false);
            if (auditEvent == null)
                return sync ? baseSaveChanges() : await baseSaveChangesAsync().ConfigureAwait(false);

            return await RunAuditedOperation(sync, auditEvent, baseSaveChanges, baseSaveChangesAsync, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Blocks on a user-supplied async delegate with SynchronizationContext temporarily removed,
        /// so awaits inside the delegate continue on the thread pool and cannot deadlock the blocked thread.
        /// </summary>
        private static void RunSync(Func<Task> action)
        {
            var prevContext = SynchronizationContext.Current;
            if (prevContext == null)
            {
                action().GetAwaiter().GetResult();
                return;
            }
            try
            {
                SynchronizationContext.SetSynchronizationContext(null);
                action().GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevContext);
            }
        }

        private async Task<int> RunAuditedOperation(bool sync, IAuditEvent auditEvent,
            Func<int> execute, Func<Task<int>> executeAsync, CancellationToken cancellationToken)
        {
            // If no ambient transaction exists, wrap the operation + audit persistence in one transaction
            // so that audit records are always consistent with the data they describe.
            // The own transaction is started through the execution strategy, otherwise retrying
            // strategies (e.g. EnableRetryOnFailure) throw on user-initiated transactions.
            var hasAmbientTransaction = Context.DbContext.Database.CurrentTransaction != null;
            if (hasAmbientTransaction)
                return await RunInTransaction(false).ConfigureAwait(false);

            var strategy = Context.DbContext.Database.CreateExecutionStrategy();
            return sync
                ? strategy.Execute(() => RunInTransaction(true).GetAwaiter().GetResult())
                : await strategy.ExecuteAsync(() => RunInTransaction(true)).ConfigureAwait(false);

            async Task<int> RunInTransaction(bool useOwnTransaction)
            {
                var ownTransaction = !useOwnTransaction
                    ? null
                    : sync
                        ? Context.DbContext.Database.BeginTransaction()
                        : await Context.DbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    try
                    {
                        auditEvent.Result = sync ? execute() : await executeAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        auditEvent.Success = false;
                        auditEvent.ErrorMessage = ex.ToString();
                        throw;
                    }
                    auditEvent.Success = true;
                    FillOutColumnValues(auditEvent);

                    foreach (var entry in auditEvent.Entries)
                    {
                        var eventType = GetEventType(entry.EntityType);
                        if (eventType == null)
                            // no mapping for this entity type
                            continue;

                        var entityAuditEvent = Activator.CreateInstance(eventType);
                        var mapperEventAction = GetMapperEventAction(entry.EntityType);
                        if (mapperEventAction == null)
                            // nothing to invoke for this entity type
                            continue;
                        if (sync)
                            RunSync(() => mapperEventAction(auditEvent, entry, entityAuditEvent));
                        else
                            await mapperEventAction(auditEvent, entry, entityAuditEvent).ConfigureAwait(false);
                    }

                    // Persist queued audit entities after all mapping actions completed
                    List<(object Entity, object Parent)> toSave = null;
                    lock (_queuedAuditEntities)
                    {
                        if (_queuedAuditEntities.Count > 0)
                        {
                            toSave = [.. _queuedAuditEntities];
                            _queuedAuditEntities.Clear();
                        }
                    }

                    if (toSave != null && toSave.Count > 0)
                    {
                        // First persist event entities so their keys are generated
                        var eventEntities = toSave.Where(x => x.Parent == null).Select(x => x.Entity).ToList();
                        if (eventEntities.Count > 0)
                        {
                            foreach (var ev in eventEntities)
                                Context.DbContext.Add(ev);
                            if (sync)
                                Context.PersistAuditEntries(eventEntities);
                            else
                                await Context.PersistAuditEntriesAsync(eventEntities, cancellationToken).ConfigureAwait(false);
                        }

                        // Then set AuditId on property entities (if any) and persist them
                        var propEntities = toSave.Where(x => x.Parent != null).ToList();
                        if (propEntities.Count > 0)
                        {
                            foreach (var (entity, parent) in propEntities)
                            {
                                if (parent != null)
                                {
                                    var parentIdProp = parent.GetType().GetProperty("Id");
                                    var auditIdProp = entity.GetType().GetProperty("AuditId");
                                    if (parentIdProp != null && auditIdProp != null)
                                    {
                                        var parentId = parentIdProp.GetValue(parent);
                                        auditIdProp.SetValue(entity, parentId);
                                    }
                                }
                                Context.DbContext.Add(entity);
                            }
                            if (sync)
                                Context.PersistAuditEntries(propEntities.Select(x => x.Entity));
                            else
                                await Context.PersistAuditEntriesAsync(propEntities.Select(x => x.Entity), cancellationToken).ConfigureAwait(false);
                        }
                    }

                    if (ownTransaction != null)
                    {
                        if (sync)
                            ownTransaction.Commit();
                        else
                            await ownTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
                catch
                {
                    // drop queued audit entities of the failed event so they don't leak into the next SaveChanges
                    lock (_queuedAuditEntities)
                    {
                        _queuedAuditEntities.Clear();
                    }
                    if (ownTransaction != null)
                    {
                        if (sync)
                            ownTransaction.Rollback();
                        else
                            await ownTransaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                    }
                    throw;
                }
                finally
                {
                    ownTransaction?.Dispose();
                }

                return auditEvent.Result;
            }
        }

        public Func<IAuditEvent, IEventEntry, object, Task> GetMapperEventAction(Type type)
        {
            Mappings.TryGetValue(type, out IMappingInfo map);

            if (map == null && EventCommonAction == null)
                return null;

            return async (auditEvent, entry, auditObj) =>
            {
                if (map?.EventAction != null)
                    await map.EventAction.Invoke(auditEvent, entry, auditObj).ConfigureAwait(false);

                if (EventCommonAction != null)
                    await EventCommonAction(auditEvent, entry, auditObj).ConfigureAwait(false);
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

        public void EnqueueAuditEntity(object entity, object parent = null)
        {
            if (entity == null)
                return;
            lock (_queuedAuditEntities)
            {
                _queuedAuditEntities.Add((entity, parent));
            }
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

        /// <summary>
        /// Register a custom resolver for a specific entity type. 
        /// This will be stored in the cache and take precedence over reflection-built resolver.
        /// </summary>
        public void RegisterLookupResolver(Type entityType, Func<object, string> resolver)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            LookupValueCache[entityType] = (obj) => resolver(obj);
        }

        /// <summary>
        /// Generic overload to register resolver for TEntity.
        /// </summary>
        public void RegisterLookupResolver<T>(Func<T, string> resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            RegisterLookupResolver(typeof(T), (obj) => resolver((T)obj));
        }

        #endregion IDBContextAuditor implementation

        protected IAuditEvent CreateAuditEvent()
            => CreateAuditEventCore(true).GetAwaiter().GetResult();

        protected Task<IAuditEvent> CreateAuditEventAsync()
            => CreateAuditEventCore(false);

        private async Task<IAuditEvent> CreateAuditEventCore(bool sync)
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
                    Changes = entry.State == EntityState.Modified
                        ? await GetChangesCore(sync, entry).ConfigureAwait(false)
                        : null,
                    Table = entityName.Table,
                    Schema = entityName.Schema,
                    Name = entry.Metadata.DisplayName()
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

        protected void FillOutColumnValues(IAuditEvent auditEvent)
        {
            foreach (var auditEntry in auditEvent.Entries)
            {
                var entry = auditEntry.GetEntry();                
                if (entry == null)
                    // ColumnValues already pre-populated (bulk ops)
                    continue;
                auditEntry.ColumnValues = new Dictionary<string, object>();
                var props = entry.Metadata.GetProperties();
                foreach (var prop in props)
                {
                    PropertyEntry propEntry = entry.Property(prop.Name);
                    if (IncludeProperty(entry, prop.Name))
                    {
                        object value = entry.State != EntityState.Deleted ?
                            propEntry.CurrentValue : propEntry.OriginalValue;
                        auditEntry.ColumnValues.Add(GetColumnName(prop), value);
                    }
                }
            }
        }
        protected List<IEventEntryChange> GetChanges(EntityEntry entry)
            => GetChangesCore(true, entry).GetAwaiter().GetResult();

        protected Task<List<IEventEntryChange>> GetChangesAsync(EntityEntry entry)
            => GetChangesCore(false, entry);

        private async Task<List<IEventEntryChange>> GetChangesCore(bool sync, EntityEntry entry)
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
                        result.Add(await GetPropertyChangesCore(sync, propEntry, navigations, prop).ConfigureAwait(false));
                }
            }
            return result;
        }
        protected EventEntryChange GetPropertyChanges(PropertyEntry propEntry,
            List<INavigation> navigations, IProperty prop)
            => GetPropertyChangesCore(true, propEntry, navigations, prop).GetAwaiter().GetResult();

        protected Task<EventEntryChange> GetPropertyChangesAsync(PropertyEntry propEntry,
            List<INavigation> navigations, IProperty prop)
            => GetPropertyChangesCore(false, propEntry, navigations, prop);

        private async Task<EventEntryChange> GetPropertyChangesCore(bool sync, PropertyEntry propEntry,
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
                    y => y.Name == propEntry.Metadata.Name)
                ).FirstOrDefault();
            if (navProp == null)
                return eec;
            var relatedType = navProp.ForeignKey.DependentToPrincipal.ClrType;
            var dbSet = Context.DbContext.Set(relatedType) as IQueryable<IEntity>;
            if (dbSet != null)
            {
                if (eec.NewValue != null)
                {
                    var newRelated = sync
                        ? dbSet.AsNoTracking().Where(x => x.Id.Equals(eec.NewValue)).FirstOrDefault()
                        : await dbSet.AsNoTracking().Where(x => x.Id.Equals(eec.NewValue))
                            .FirstOrDefaultAsync().ConfigureAwait(false);
                    if (newRelated != null)
                        eec.NewValue = GetLookupValue(newRelated);
                }
                if (eec.OriginalValue != null)
                {
                    var oldRelated = sync
                        ? dbSet.AsNoTracking().Where(x => x.Id.Equals(eec.OriginalValue)).FirstOrDefault()
                        : await dbSet.AsNoTracking().Where(x => x.Id.Equals(eec.OriginalValue))
                            .FirstOrDefaultAsync().ConfigureAwait(false);
                    if (oldRelated != null)
                        eec.OriginalValue = GetLookupValue(oldRelated);
                }
            }
            return eec;
        }

        protected string GetLookupValue(object entity)
        {
            if (entity == null)
                return null;

            // If a global resolver is provided, use it first (allows user-supplied override)
            if (LookupValueResolver != null)
            {
                try
                {
                    var resolved = LookupValueResolver(entity);
                    if (resolved != null)
                        return resolved;
                }
                catch
                {
                    // swallow resolver exceptions and fall back to built-in behavior
                }
            }

            var entityType = entity.GetType();

            // Check if we have a cached lookup builder for this type (or a previously registered custom resolver)
            if (!LookupValueCache.TryGetValue(entityType, out var lookupBuilder))
            {
                lookupBuilder = BuildLookupValueBuilder(entityType);
                LookupValueCache.TryAdd(entityType, lookupBuilder);
            }

            return lookupBuilder?.Invoke(entity);
        }

        protected Func<object, string> BuildLookupValueBuilder(Type entityType)
        {
            // Try to get Id property through IEntity interface
            var idProp = entityType.GetProperty("Id",
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            // Try to get Title property through ITitleEntity interface
            var titleProp = entityType.GetProperty("Title",
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (idProp == null)
                return null;

            return (entity) =>
            {
                try
                {
                    var idValue = idProp.GetValue(entity);
                    var titleValue = titleProp?.GetValue(entity) as string;

                    if (string.IsNullOrEmpty(titleValue))
                        return idValue?.ToString() ?? "(null)";

                    return $"{idValue}: {titleValue}";
                }
                catch
                {
                    return null;
                }
            };
        }

        protected bool IncludeProperty(EntityEntry entry, string propName)
        {
            var entityType = GetDefiningType(entry)?.ClrType;
            if (entityType == null)
                return true;
            return IncludeProperty(entityType, propName);
        }

        protected bool IncludeProperty(Type entityType, string propName)
        {
            var ignoredProperties = EnsurePropertiesIgnoreAttrCache(entityType);
            if (ignoredProperties != null && ignoredProperties.Contains(propName))
                return false;
            if (GlobalIgnoredProperties != null
                && GlobalIgnoredProperties.ContainsKey(propName))
                return false;
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

        protected IDictionary<string, object> GetBulkEntityColumnValues(object entity, Type clrType)
        {
            var efEntityType = Context.DbContext.Model.FindEntityType(clrType);
            if (efEntityType == null)
                return new Dictionary<string, object>();
            var result = new Dictionary<string, object>();
            foreach (var prop in efEntityType.GetProperties())
            {
                if (prop.PropertyInfo == null)
                    continue;
                if (!IncludeProperty(clrType, prop.Name))
                    continue;
                result[GetColumnName(prop)] = prop.PropertyInfo.GetValue(entity);
            }
            return result;
        }

        protected string GetColumnNameForProperty(Type entityType, string clrPropertyName)
        {
            var efEntityType = Context.DbContext.Model.FindEntityType(entityType);
            if (efEntityType == null)
                return null;
            var prop = efEntityType.FindProperty(clrPropertyName);
            if (prop == null)
                return null;
            return GetColumnName(prop);
        }

        public int AuditBulkOperation(Type entityType, int action, IList<object> entities, Func<int> execute)
            => AuditBulkOperationCore(true, entityType, action, entities, null, null, execute, CancellationToken.None)
                .GetAwaiter().GetResult();

        public Task<int> AuditBulkOperationAsync(Type entityType, int action, IList<object> entities,
            Func<Task<int>> execute, CancellationToken cancellationToken = default)
            => AuditBulkOperationCore(false, entityType, action, entities, null, execute, null, cancellationToken);

        public int AuditBulkOperation(Type entityType, int action, IList<object> entities,
            Func<object, IDictionary<string, object>> newValueExtractor, Func<int> execute)
            => AuditBulkOperationCore(true, entityType, action, entities, newValueExtractor, null, execute, CancellationToken.None)
                .GetAwaiter().GetResult();

        public Task<int> AuditBulkOperationAsync(Type entityType, int action, IList<object> entities,
            Func<object, IDictionary<string, object>> newValueExtractor,
            Func<Task<int>> execute, CancellationToken cancellationToken = default)
            => AuditBulkOperationCore(false, entityType, action, entities, newValueExtractor, execute, null, cancellationToken);

        private async Task<int> AuditBulkOperationCore(bool sync, Type entityType, int action,
            IList<object> entities, Func<object, IDictionary<string, object>> newValueExtractor,
            Func<Task<int>> executeAsync, Func<int> execute,
            CancellationToken cancellationToken)
        {
            if (!Enabled || GetEventType(entityType) == null || entities == null || entities.Count == 0)
                return sync ? execute() : await executeAsync().ConfigureAwait(false);

            var efEntityType = Context.DbContext.Model.FindEntityType(entityType);
            var auditEvent = new AuditEvent
            {
                Entries = entities.Select(e =>
                {
                    var columnValues = GetBulkEntityColumnValues(e, entityType);
                    IList<IEventEntryChange> changes = null;
                    if (newValueExtractor != null)
                    {
                        var newValues = newValueExtractor(e);
                        if (newValues != null)
                        {
                            changes = new List<IEventEntryChange>();
                            foreach (var kv in newValues)
                            {
                                if (!IncludeProperty(entityType, kv.Key))
                                    continue;
                                var colName = GetColumnNameForProperty(entityType, kv.Key);
                                if (colName == null)
                                    continue;
                                changes.Add(new EventEntryChange
                                {
                                    ColumnName = colName,
                                    OriginalValue = columnValues.TryGetValue(colName, out var old) ? old : null,
                                    NewValue = kv.Value
                                });
                            }
                        }
                    }
                    return (IEventEntry)new EventEntry
                    {
                        EntityType = entityType,
                        Action = action,
                        Name = efEntityType?.DisplayName(),
                        Table = efEntityType?.GetTableName(),
                        Schema = efEntityType?.GetSchema(),
                        Entry = null,
                        ColumnValues = columnValues,
                        Changes = changes
                    };
                }).ToList(),
                ContextId = Context.DbContext.ContextId.ToString()
            };

            return await RunAuditedOperation(sync, auditEvent, execute, executeAsync, cancellationToken).ConfigureAwait(false);
        }
    }
}
