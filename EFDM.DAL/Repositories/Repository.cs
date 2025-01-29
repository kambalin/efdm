using EFDM.Abstractions.DAL.Repositories;
using EFDM.Abstractions.DataQueries;
using EFDM.Abstractions.Models.Responses;
using EFDM.Core.DAL.Providers;
using EFDM.Core.Extensions;
using EFDM.Core.Models.Domain;
using EFDM.Core.Models.Responses;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EFDM.Core.DAL.Repositories
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : IdKeyEntityBase<TKey>, new()
        where TKey : IComparable, IEquatable<TKey>
    {
        #region fields & properties

        public virtual EFDMDatabaseContext Context { get; }
        public DbSet<TEntity> DbSet { get; protected set; }
        public bool AutoDetectChanges { get; set; } = true;
        public virtual int ExecutorId { get { return Context.ExecutorId; } }
        public bool AutoDetectChangesEnabled
        {
            get { return Context.ChangeTracker.AutoDetectChangesEnabled; }
            set { Context.ChangeTracker.AutoDetectChangesEnabled = value; }
        }

        #endregion fields & properties

        #region constructors

        public Repository(EFDMDatabaseContext dbContext)
        {
            Context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            InitDbSet();
        }

        #endregion constructors

        protected virtual void InitDbSet()
        {
            if (Context != null)
                DbSet = Context.Set<TEntity>();
        }

        #region IRepository implementation

        public async Task<IEnumerable<TEntity>> FetchAsync(IDataQuery<TEntity> query, bool tracking = false,
            CancellationToken cancellationToken = default)
        {
            var dbQuery = FetchPrepare(query, tracking);
            return await dbQuery.ToListAsync(cancellationToken);
        }

        public async Task<IPagedList<TEntity>> FetchPagedAsync(IDataQuery<TEntity> query, bool tracking = false,
            CancellationToken cancellationToken = default)
        {
            var result = new PagedList<TEntity>
            {
                TotalCount = await CountAsync(query, cancellationToken),
                Skipped = query?.Skip ?? 0
            };

            var dbQuery = FetchPrepare(query, tracking);
            result.Items = await dbQuery.ToListAsync();
            return result;
        }

        public async Task<IEnumerable<TEntity>> FetchLiteAsync(IDataQuery<TEntity> query,
            Expression<Func<TEntity, TEntity>> select, bool tracking = false,
            CancellationToken cancellationToken = default)
        {
            var dbQuery = FetchPrepare(query, tracking);
            IEnumerable<TEntity> entities;
            if (select != null)
                entities = await dbQuery.Select(select).ToListAsync(cancellationToken);
            else
                entities = await dbQuery.ToListAsync(cancellationToken);
            if (tracking)
                DbSet.AttachRange(entities);
            return entities;
        }

        public async Task<IEnumerable<TKey>> FetchIdsAsync(IDataQuery<TEntity> query,
            CancellationToken cancellationToken = default)
        {
            var dbQuery = FetchPrepare(query, false);
            Expression<Func<TEntity, TEntity>> selector = x => new TEntity { Id = x.Id };
            var result = await dbQuery.Select(selector).ToListAsync(cancellationToken);
            return result.Select(x => x.Id);
        }

        public virtual async Task<int> CountAsync(IDataQuery<TEntity> query = null,
            CancellationToken cancellationToken = default)
        {
            var dbQuery = CountPrepare(query);
            return await dbQuery.CountAsync(cancellationToken);
        }

        public virtual async Task AddAsync(params TEntity[] entities)
        {
            if (entities?.Any() != true)
                return;
            using (new ActionExecutor(Context, AutoDetectChanges))
            {
                await DbSet.AddRangeAsync(entities);
            }
        }

        public virtual void Delete(params TEntity[] entities)
        {
            if (entities?.Any() != true)
                return;
            using (new ActionExecutor(Context, AutoDetectChanges))
            {
                DbSet.RemoveRange(entities);
            }
        }

        public virtual async Task<int> ExecuteDeleteAsync(IDataQuery<TEntity> query,
            CancellationToken cancellationToken = default)
        {
            var dbQuery = FilterByQuery(DbSet.AsQueryable(), query);
            return await dbQuery.ExecuteDeleteAsync(cancellationToken);
        }

        public virtual async Task<int> ExecuteUpdateAsync(IDataQuery<TEntity> query,
            Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls,
            CancellationToken cancellationToken = default)
        {
            var dbQuery = FilterByQuery(DbSet.AsQueryable(), query);
            return await dbQuery.ExecuteUpdateAsync(setPropertyCalls, cancellationToken);
        }

        public virtual void Update(params TEntity[] entities)
        {
            if (entities?.Any() != true)
                return;
            Context.UpdateRange(entities);
        }

        public virtual async Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            bool isEntityDetached = Context.Entry(entity).State == EntityState.Detached;
            TEntity attachedEntity = null;
            if (isEntityDetached)
            {
                var entityProperties = GetEntityProperties(entity);
                attachedEntity = DbSet.Local.Where(e => e.Id.Equals(entity.Id)).FirstOrDefault();
                if (attachedEntity == null)
                {
                    var dbQuery = DbSet.Where(e => e.Id.Equals(entity.Id)).AsQueryable();
                    foreach (var pi in entityProperties.ColProps)
                        dbQuery = dbQuery.Include(pi.Name);
                    attachedEntity = await dbQuery.FirstOrDefaultAsync(cancellationToken);
                    if (attachedEntity == null)
                        throw new Exception($"Entity is detached, cannot find entity with Id = '{entity.Id}'");
                }

                var entry = Context.Entry(attachedEntity);
                entry.CurrentValues.SetValues(entity);

                foreach (var property in entityProperties.ColProps)
                {
                    SetValueCollectionNavProp(property, entry, attachedEntity, entity);
                }

                foreach (var property in entityProperties.NotColProps)
                {
                    SetValueNotCollectionNavProp(property, attachedEntity, entity);
                }
            }
            await SaveChangesAsync(cancellationToken);
            return !isEntityDetached ? entity : attachedEntity;
        }

        public virtual async Task<TEntity> SaveAsync(TEntity model, CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] updateProperties)
        {
            var type = typeof(TEntity);
            var modified = false;
            var entityProperties = GetEntityProperties(model);
            bool isEntityDetached = Context.Entry(model).State == EntityState.Detached;
            if (!isEntityDetached)
                Context.Entry(model).State = EntityState.Detached;

            var dbQuery = DbSet.Where(e => e.Id.Equals(model.Id)).AsQueryable();
            foreach (var pi in entityProperties.ColProps)
                dbQuery = dbQuery.Include(pi.Name);
            var entity = await dbQuery.FirstOrDefaultAsync(cancellationToken);
            if (entity == null)
                throw new Exception($"Cannot find entity with Id = '{model.Id}'");
            var entry = Context.Entry(entity);

            foreach (var up in updateProperties)
            {
                MemberExpression memberExpression;
                if (up.Body is UnaryExpression)
                {
                    var unaryExpression = (UnaryExpression)up.Body;
                    memberExpression = (MemberExpression)unaryExpression.Operand;
                }
                else
                    memberExpression = (MemberExpression)up.Body;
                var memberName = memberExpression.Member.Name;
                var property = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.Name == memberName).FirstOrDefault();

                if (!entityProperties.NavProps.Any(x => x.Name == memberName))
                {
                    property.SetValue(entity, model.GetType()
                        .GetProperty(property.Name)
                        .GetValue(model, null)
                    );
                    entry.Property(up).IsModified = true;
                }
                else
                {
                    if (entityProperties.NotColProps.Any(x => x.Name == memberName))
                        SetValueNotCollectionNavProp(property, entity, model);
                    else
                        SetValueCollectionNavProp(property, entry, entity, model);
                }
                modified = true;
            }
            if (modified)
                entry.State = EntityState.Modified;

            await Context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await Context.SaveChangesAsync(cancellationToken);
        }

        public virtual void ClearChangeTracker()
        {
            Context.ClearChangeTracker();
        }

        public virtual IQueryable<TEntity> QueryableSql(string sql, params object[] parameters)
            => DbSet.FromSqlRaw(sql, parameters);

        public virtual bool IsAttached(TKey id)
        {
            return DbSet.Local.Any(e => e.Id.Equals(id));
        }

        #endregion IRepository implementation

        private IQueryable<TEntity> FetchPrepare(IDataQuery<TEntity> query, bool tracking = false)
        {
            var dbQuery = tracking ? DbSet.AsQueryable() : DbSet.AsNoTracking().AsQueryable();
            dbQuery = ApplyQuery(dbQuery, query);
            return dbQuery;
        }

        private IQueryable<TEntity> CountPrepare(IDataQuery<TEntity> query, bool tracking = false)
        {
            var dbQuery = DbSet.AsNoTracking().AsQueryable();
            if (query != null)
                dbQuery = FilterByQuery(dbQuery, query);
            return dbQuery;
        }

        private void SetValueNotCollectionNavProp(PropertyInfo property, TEntity entity, TEntity model)
        {
            var type = typeof(TEntity);
            var value = property.GetValue(model);
            // get id value, if object has it
            var id = property.PropertyType.GetProperty("Id")?.GetValue(value);
            // get value, if object follow name convention like StatusId for Status, where StatusId is Foreign Key
            var piId = type.GetProperty($"{property.Name}Id");

            if (id != null && piId != null) // if property is a class for FK
                piId.SetValue(entity, id);
            else
            {
                if (property.PropertyType == type && id?.Equals(type.GetProperty("Id").GetValue(model)) == true)
                    property.SetValue(entity, model);
                else
                    property.SetValue(entity, value);
            }
        }

        private void SetValueCollectionNavProp(PropertyInfo property, EntityEntry<TEntity> entry, TEntity entity, TEntity model)
        {
            var propertyName = property.Name;
            var dbItemsEntry = entry.Collection(propertyName);
            var accessor = dbItemsEntry.Metadata.GetCollectionAccessor();
            dbItemsEntry.Load();
            var dbItemsMap = ((IEnumerable<object>)dbItemsEntry.CurrentValue)
                .ToDictionary(x => string.Join("|", FindPrimaryKeyValues(x)));
            var items = (IEnumerable<object>)accessor.GetOrCreate(model, false);
            foreach (var item in items)
            {
                var key = string.Join("|", FindPrimaryKeyValues(item));
                if (!dbItemsMap.TryGetValue(key, out var oldItem))
                    accessor.Add(entity, item, false);
                else
                {
                    Context.Entry(oldItem).CurrentValues.SetValues(item);
                    dbItemsMap.Remove(key);
                }
            }
            foreach (var oldItem in dbItemsMap.Values)
                accessor.Remove(entity, oldItem);
        }
        /// <summary>
        /// Return navigation properties
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Tuple - all nav properties, collection nav properties, objects nav properties</returns>
        private (PropertyInfo[] NavProps, IEnumerable<PropertyInfo> ColProps, IEnumerable<PropertyInfo> NotColProps) GetEntityProperties(TEntity model)
        {
            var navigationProps = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanRead && x.CanWrite && _isIncludable(x.PropertyType) && x.GetValue(model) != null)
                .ToArray();
            var collectionProps = navigationProps.Where(x => typeof(IEnumerable).IsAssignableFrom(x.PropertyType));
            var notCollectionProps = navigationProps.Where(x => !typeof(IEnumerable).IsAssignableFrom(x.PropertyType));
            return (navigationProps, collectionProps, notCollectionProps);
        }

        protected virtual IQueryable<TEntity> ApplyQuery(IQueryable<TEntity> dbQuery, IDataQuery<TEntity> query)
        {
            if (query == null)
                return dbQuery;

            if (query.SplitQuery)
                dbQuery = dbQuery.AsSplitQuery();
            dbQuery = IncludeByQuery(dbQuery, query);
            dbQuery = FilterByQuery(dbQuery, query);
            dbQuery = SortByQuery(dbQuery, query);
            dbQuery = PageByQuery(dbQuery, query);
            return dbQuery;
        }

        IEnumerable<object> FindPrimaryKeyValues(object entity)
        {
            return Context.Model.FindEntityType(entity.GetType()).FindPrimaryKey().Properties
                .Select(x => x.PropertyInfo.GetValue(entity));
        }

        protected virtual IQueryable<TEntity> FilterByQuery(IQueryable<TEntity> dbQuery, IDataQuery<TEntity> query)
        {
            var expr = query.ToFilter().ToExpression();
            return expr != null ? dbQuery.AsExpandable().Where(expr) : dbQuery;
        }

        protected virtual IQueryable<TEntity> IncludeByQuery(IQueryable<TEntity> dbQuery, IDataQuery<TEntity> query)
        {
            return query.Includes?.Aggregate(dbQuery, (current, include) => current.Include(include)) ?? dbQuery;
        }

        protected virtual IQueryable<TEntity> PageByQuery(IQueryable<TEntity> dbQuery, IDataQuery<TEntity> query)
        {
            if (query.Skip > 0)
                dbQuery = dbQuery.Skip(query.Skip);
            if (query.Take > 0)
                dbQuery = dbQuery.Take(query.Take);
            return dbQuery;
        }

        protected virtual IQueryable<TEntity> SortByQuery(IQueryable<TEntity> dbQuery, IDataQuery<TEntity> query)
        {
            if (query.Sorts == null || query.Sorts.Count() == 0)
            {
                dbQuery = _orderBy(dbQuery, nameof(IdKeyEntityBase<TKey>.Id), true, false);
                return dbQuery;
            }

            var ordered = false;
            foreach (var sort in query.Sorts)
            {
                dbQuery = _orderBy(dbQuery, sort.Field, sort.Desc, ordered);
                ordered = true;
            }
            return dbQuery;
        }

        static IOrderedQueryable<T> _orderBy<T>(IQueryable<T> source, string name, bool descending, bool then)
        {
            var param = Expression.Parameter(typeof(T), string.Empty);
            var property = name.Split('.')
                .Aggregate<string, Expression>(param, (r, x) => Expression.PropertyOrField(r, x));
            var sort = Expression.Lambda(property, param);
            var call = Expression.Call(
                typeof(Queryable),
                (!then ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
                new[] { typeof(T), property.Type },
                source.Expression,
                Expression.Quote(sort));
            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
        }

        bool _isIncludable(Type type)
        {
            return (type.IsClass || typeof(IEnumerable).IsAssignableFrom(type)) && !_notIncludable.Contains(type);
        }

        HashSet<Type> _notIncludable = new HashSet<Type> { typeof(string), typeof(byte[]) };

        internal class ActionExecutor : IDisposable
        {
            private EFDMDatabaseContext _context;
            private bool _autoDetectChanges;

            public ActionExecutor(EFDMDatabaseContext context, bool autoDetectChanges)
            {
                _context = context;
                _autoDetectChanges = autoDetectChanges;

                if (!_autoDetectChanges)
                    _context.ChangeTracker.AutoDetectChangesEnabled = false;
            }

            public void Dispose()
            {
                if (!_autoDetectChanges)
                    _context.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }
    }
}
