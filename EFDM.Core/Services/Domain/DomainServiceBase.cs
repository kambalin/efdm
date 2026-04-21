using EFDM.Abstractions.DAL.Repositories;
using EFDM.Abstractions.DataQueries;
using EFDM.Abstractions.Models.Domain;
using EFDM.Abstractions.Models.Responses;
using EFDM.Abstractions.Models.Validation;
using EFDM.Abstractions.Services.Domain;
using EFDM.Core.Models.Validation;
using EFDM.Core.Services.Base;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EFDM.Core.Services.Domain
{
    public abstract class DomainServiceBase<TModel, TQuery, TKey, TRepository> : ServiceBase,
        IDomainService<TModel, TQuery, TKey>
        where TModel : class, IIdKeyEntity<TKey>, new()
        where TKey : IComparable, IEquatable<TKey>
        where TQuery : class, IDataQuery<TModel, TKey>
        where TRepository : IRepository<TModel, TKey>
    {
        #region fields & properties

        public virtual int ExecutorId
        {
            get
            {
                return Repository.ExecutorId;
            }
        }
        public bool AutoDetectChangesEnabled
        {
            get { return Repository.AutoDetectChangesEnabled; }
            set { Repository.AutoDetectChangesEnabled = value; }
        }
        protected readonly TRepository Repository;
        private Expression<Func<TModel, TModel>> _liteSelector = x => new TModel() { Id = x.Id };
        protected Expression<Func<TModel, TModel>> LiteSelector
        {
            get
            {
                if (_liteSelector == null)
                    throw new NullReferenceException($"Need to initialize '{nameof(LiteSelector)}' in domain service class");
                return _liteSelector;
            }
            set
            {
                _liteSelector = value;
            }
        }

        #endregion fields & properties

        #region constructors

        protected DomainServiceBase(TRepository repository, ILogger logger) : base(logger)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #endregion constructors

        #region IDomainService implementation

        public virtual async Task<IEnumerable<TModel>> FetchAsync(TQuery query = null, bool tracking = false,
            CancellationToken cancellationToken = default)
        {
            var result = await Repository.FetchAsync(query, tracking, cancellationToken);
            return result;
        }

        public virtual IEnumerable<TModel> Fetch(TQuery query = null, bool tracking = false)
            => Repository.Fetch(query, tracking);

        public virtual async Task<IPagedList<TModel>> FetchPagedAsync(TQuery query = null,
            CancellationToken cancellationToken = default)
        {
            var result = await Repository.FetchPagedAsync(query, false, cancellationToken);
            return result;
        }

        public virtual IPagedList<TModel> FetchPaged(TQuery query = null)
            => Repository.FetchPaged(query, false);

        public virtual async Task<IEnumerable<TModel>> FetchLiteAsync(TQuery query,
            Expression<Func<TModel, TModel>> select,
            bool tracking = false, CancellationToken cancellationToken = default)
        {
            var result = await Repository.FetchLiteAsync(query, select, tracking, cancellationToken);
            return result;
        }

        public virtual IEnumerable<TModel> FetchLite(TQuery query, Expression<Func<TModel, TModel>> select,
            bool tracking = false)
            => Repository.FetchLite(query, select, tracking);

        public virtual async Task<IEnumerable<TModel>> FetchLiteAsync(TQuery query, bool tracking = false,
            CancellationToken cancellationToken = default)
        {
            return await FetchLiteAsync(query, LiteSelector, tracking, cancellationToken);
        }

        public virtual IEnumerable<TModel> FetchLite(TQuery query, bool tracking = false)
            => FetchLite(query, LiteSelector, tracking);

        public virtual async Task<int> CountAsync(TQuery query = null, CancellationToken cancellationToken = default)
        {
            var result = await Repository.CountAsync(query);
            return result;
        }

        public virtual int Count(TQuery query = null)
            => Repository.Count(query);

        public virtual async Task AddAsync(params TModel[] models)
        {
            await Repository.AddAsync(models);
        }

        public virtual void Add(params TModel[] models)
            => Repository.Add(models);

        public virtual async Task<TModel> SaveAsync(TModel model, CancellationToken cancellationToken = default)
            => await SaveCore(false, model, cancellationToken);

        public virtual TModel Save(TModel model)
            => SaveCore(true, model, CancellationToken.None).GetAwaiter().GetResult();

        private async Task<TModel> SaveCore(bool sync, TModel model, CancellationToken cancellationToken)
        {
            var isNew = model.Id.Equals(default);
            if (isNew)
            {
                if (sync)
                    Repository.Add(model);
                else
                    await Repository.AddAsync(model);
            }
            return sync ? Repository.Save(model) : await Repository.SaveAsync(model, cancellationToken);
        }

        public virtual async Task DeleteAsync(TKey id, bool forceDeleteEvenDeletable = false,
            CancellationToken cancellationToken = default)
            => await DeleteCore(false, id, forceDeleteEvenDeletable, cancellationToken);

        public virtual void Delete(TKey id, bool forceDeleteEvenDeletable = false)
            => DeleteCore(true, id, forceDeleteEvenDeletable, CancellationToken.None).GetAwaiter().GetResult();

        private async Task DeleteCore(bool sync, TKey id, bool forceDeleteEvenDeletable, CancellationToken cancellationToken)
        {
            var entity = sync ? GetById(id, true) : await GetByIdAsync(id, true, null, cancellationToken);
            await DeleteCore(sync, entity, forceDeleteEvenDeletable, cancellationToken);
        }

        public virtual async Task DeleteAsync(IEnumerable<TModel> entities, bool forceDeleteEvenDeletable = false,
            CancellationToken cancellationToken = default)
            => await DeleteCore(false, entities, forceDeleteEvenDeletable, cancellationToken);

        public virtual void Delete(IEnumerable<TModel> entities, bool forceDeleteEvenDeletable = false)
            => DeleteCore(true, entities, forceDeleteEvenDeletable, CancellationToken.None).GetAwaiter().GetResult();

        private async Task DeleteCore(bool sync, IEnumerable<TModel> entities, bool forceDeleteEvenDeletable,
            CancellationToken cancellationToken)
        {
            foreach (var entity in entities)
                await DeleteCore(sync, entity, forceDeleteEvenDeletable, cancellationToken);
        }

        public virtual async Task DeleteAsync(TModel entity, bool forceDeleteEvenDeletable = false,
            CancellationToken cancellationToken = default)
            => await DeleteCore(false, entity, forceDeleteEvenDeletable, cancellationToken);

        public virtual void Delete(TModel entity, bool forceDeleteEvenDeletable = false)
            => DeleteCore(true, entity, forceDeleteEvenDeletable, CancellationToken.None).GetAwaiter().GetResult();

        private async Task DeleteCore(bool sync, TModel entity, bool forceDeleteEvenDeletable,
            CancellationToken cancellationToken)
        {
            var deletable = entity as IDeletableEntity;
            if (deletable == null || forceDeleteEvenDeletable)
                Repository.Delete(entity);
            else if (!deletable.IsDeleted)
                deletable.IsDeleted = true;
            if (sync)
                SaveChanges();
            else
                await SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<int> ExecuteDeleteAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            var res = await Repository.ExecuteDeleteAsync(query, cancellationToken);
            return res;
        }

        public virtual int ExecuteDelete(TQuery query)
            => Repository.ExecuteDelete(query);

        public virtual async Task<int> ExecuteUpdateAsync(IDataQuery<TModel> query,
            Expression<Func<SetPropertyCalls<TModel>, SetPropertyCalls<TModel>>> setPropertyCalls,
            CancellationToken cancellationToken = default)
        {
            return await Repository.ExecuteUpdateAsync(query, setPropertyCalls, cancellationToken);
        }

        public virtual int ExecuteUpdate(IDataQuery<TModel> query,
            Expression<Func<SetPropertyCalls<TModel>, SetPropertyCalls<TModel>>> setPropertyCalls)
            => Repository.ExecuteUpdate(query, setPropertyCalls);

        public virtual async Task<TModel> GetAsync(TQuery query, bool tracking = false,
            CancellationToken cancellationToken = default)
            => await GetCore(false, query, tracking, cancellationToken);

        public virtual TModel Get(TQuery query, bool tracking = false)
            => GetCore(true, query, tracking, CancellationToken.None).GetAwaiter().GetResult();

        private async Task<TModel> GetCore(bool sync, TQuery query, bool tracking, CancellationToken cancellationToken)
        {
            query.Take = 1;
            var result = sync ? Repository.Fetch(query, tracking) : await Repository.FetchAsync(query, tracking, cancellationToken);
            return result.FirstOrDefault();
        }

        public virtual async Task<TModel> GetByIdAsync(TKey key, bool tracking = false,
            IEnumerable<string> includes = null, CancellationToken cancellationToken = default)
        {
            var keys = new TKey[] { key };
            return (await GetByIdsAsync(keys, tracking, includes, cancellationToken)).FirstOrDefault();
        }

        public virtual TModel GetById(TKey key, bool tracking = false, IEnumerable<string> includes = null)
            => GetByIds(new[] { key }, tracking, includes).FirstOrDefault();

        public virtual async Task<List<TModel>> GetByIdsAsync(IEnumerable<TKey> keys, bool tracking = false,
            IEnumerable<string> includes = null, CancellationToken cancellationToken = default)
            => await GetByIdsCore(false, keys, tracking, includes, cancellationToken);

        public virtual List<TModel> GetByIds(IEnumerable<TKey> keys, bool tracking = false,
            IEnumerable<string> includes = null)
            => GetByIdsCore(true, keys, tracking, includes, CancellationToken.None).GetAwaiter().GetResult();

        private async Task<List<TModel>> GetByIdsCore(bool sync, IEnumerable<TKey> keys, bool tracking,
            IEnumerable<string> includes, CancellationToken cancellationToken)
        {
            var query = Activator.CreateInstance<TQuery>();
            query.Ids = keys.ToArray();
            if (keys.Count() == 1)
                query.Take = 1;
            if (includes != null)
                query.Includes = includes;
            var result = sync ? Repository.Fetch(query, tracking) : await Repository.FetchAsync(query, tracking);
            return result.ToList();
        }

        public virtual async Task<TModel> GetLiteAsync(TQuery query, Expression<Func<TModel, TModel>> select,
            bool tracking = false, CancellationToken cancellationToken = default)
            => await GetLiteCore(false, query, select, tracking, cancellationToken);

        public virtual TModel GetLite(TQuery query, Expression<Func<TModel, TModel>> select, bool tracking = false)
            => GetLiteCore(true, query, select, tracking, CancellationToken.None).GetAwaiter().GetResult();

        private async Task<TModel> GetLiteCore(bool sync, TQuery query, Expression<Func<TModel, TModel>> select,
            bool tracking, CancellationToken cancellationToken)
        {
            query.Take = 1;
            var result = sync
                ? Repository.FetchLite(query, select, tracking)
                : await Repository.FetchLiteAsync(query, select, tracking, cancellationToken);
            return result.FirstOrDefault();
        }

        public virtual async Task<TModel> GetLiteByIdAsync(TKey key, Expression<Func<TModel, TModel>> select,
            bool tracking = false, IEnumerable<string> includes = null, CancellationToken cancellationToken = default)
        {
            var keys = new TKey[] { key };
            return (await GetLiteByIdsAsync(keys, select, tracking, includes, cancellationToken)).FirstOrDefault();
        }

        public virtual TModel GetLiteById(TKey key, Expression<Func<TModel, TModel>> select, bool tracking = false,
            IEnumerable<string> includes = null)
            => GetLiteByIds(new[] { key }, select, tracking, includes).FirstOrDefault();

        public virtual async Task<List<TModel>> GetLiteByIdsAsync(IEnumerable<TKey> keys,
            Expression<Func<TModel, TModel>> select,
            bool tracking = false, IEnumerable<string> includes = null, CancellationToken cancellationToken = default)
            => await GetLiteByIdsCore(false, keys, select, tracking, includes, cancellationToken);

        public virtual List<TModel> GetLiteByIds(IEnumerable<TKey> keys, Expression<Func<TModel, TModel>> select,
            bool tracking = false, IEnumerable<string> includes = null)
            => GetLiteByIdsCore(true, keys, select, tracking, includes, CancellationToken.None).GetAwaiter().GetResult();

        private async Task<List<TModel>> GetLiteByIdsCore(bool sync, IEnumerable<TKey> keys,
            Expression<Func<TModel, TModel>> select, bool tracking, IEnumerable<string> includes,
            CancellationToken cancellationToken)
        {
            var query = Activator.CreateInstance<TQuery>();
            query.Ids = keys.ToArray();
            if (keys.Count() == 1)
                query.Take = 1;
            if (includes != null)
                query.Includes = includes;
            var result = sync
                ? Repository.FetchLite(query, select, tracking)
                : await Repository.FetchLiteAsync(query, select, tracking, cancellationToken);
            return result.ToList();
        }

        public virtual async Task<TModel> GetLiteAsync(TQuery query, bool tracking = false,
            CancellationToken cancellationToken = default)
        {
            return await GetLiteAsync(query, LiteSelector, tracking, cancellationToken);
        }

        public virtual TModel GetLite(TQuery query, bool tracking = false)
            => GetLite(query, LiteSelector, tracking);

        public virtual async Task<TModel> GetLiteByIdAsync(TKey key, bool tracking = false,
            IEnumerable<string> includes = null, CancellationToken cancellationToken = default)
        {
            return await GetLiteByIdAsync(key, LiteSelector, tracking, includes, cancellationToken);
        }

        public virtual TModel GetLiteById(TKey key, bool tracking = false, IEnumerable<string> includes = null)
            => GetLiteById(key, LiteSelector, tracking, includes);

        public virtual async Task<List<TModel>> GetLiteByIdsAsync(IEnumerable<TKey> keys, bool tracking = false,
            IEnumerable<string> includes = null, CancellationToken cancellationToken = default)
        {
            return await GetLiteByIdsAsync(keys, LiteSelector, tracking, includes, cancellationToken);
        }

        public virtual List<TModel> GetLiteByIds(IEnumerable<TKey> keys, bool tracking = false,
            IEnumerable<string> includes = null)
            => GetLiteByIds(keys, LiteSelector, tracking, includes);

        public virtual async Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default)
            => await ExistsCore(false, key, cancellationToken);

        public virtual bool Exists(TKey key)
            => ExistsCore(true, key, CancellationToken.None).GetAwaiter().GetResult();

        private async Task<bool> ExistsCore(bool sync, TKey key, CancellationToken cancellationToken)
        {
            var query = Activator.CreateInstance<TQuery>();
            query.Ids = new[] { key };
            return sync ? Repository.Count(query) > 0 : await Repository.CountAsync(query, cancellationToken) > 0;
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await Repository.SaveChangesAsync(cancellationToken);
        }

        public virtual int SaveChanges()
            => Repository.SaveChanges();

        public virtual IValidationResult DeleteValidation(TModel model)
        {
            return new ValidationResult();
        }

        public virtual bool IsCreator(TModel model, int userId)
        {
            var auditable = model as IAuditablePrincipalEntity;
            if (model != null)
                return auditable.CreatedById == userId;
            return false;
        }

        public virtual void ClearChangeTracker()
        {
            Repository.ClearChangeTracker();
        }

        public async Task<IEnumerable<TKey>> FetchIdsAsync(IDataQuery<TModel> query,
            CancellationToken cancellationToken = default)
        {
            return await Repository.FetchIdsAsync(query);
        }

        public IEnumerable<TKey> FetchIds(IDataQuery<TModel> query)
            => Repository.FetchIds(query);

        #endregion IDomainService implementation
    }
}
