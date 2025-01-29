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

        public virtual async Task<IPagedList<TModel>> FetchPagedAsync(TQuery query = null,
            CancellationToken cancellationToken = default)
        {
            var result = await Repository.FetchPagedAsync(query, false, cancellationToken);
            return result;
        }

        public virtual async Task<IEnumerable<TModel>> FetchLiteAsync(TQuery query,
            Expression<Func<TModel, TModel>> select,
            bool tracking = false, CancellationToken cancellationToken = default)
        {
            var result = await Repository.FetchLiteAsync(query, select, tracking, cancellationToken);
            return result;
        }

        public virtual async Task<IEnumerable<TModel>> FetchLiteAsync(TQuery query, bool tracking = false,
            CancellationToken cancellationToken = default)
        {
            return await FetchLiteAsync(query, LiteSelector, tracking, cancellationToken);
        }

        public virtual async Task<int> CountAsync(TQuery query = null, CancellationToken cancellationToken = default)
        {
            var result = await Repository.CountAsync(query);
            return result;
        }

        public virtual async Task AddAsync(params TModel[] models)
        {
            await Repository.AddAsync(models);
        }

        public virtual async Task<TModel> SaveAsync(TModel model, CancellationToken cancellationToken = default)
        {
            var isNew = model.Id.Equals(default);
            if (isNew)
                await Repository.AddAsync(model);
            return await Repository.SaveAsync(model);
        }

        public virtual async Task DeleteAsync(TKey id, bool forceDeleteEvenDeletable = false,
            CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, true, null, cancellationToken);
            await DeleteAsync(entity, forceDeleteEvenDeletable, cancellationToken);
        }

        public virtual async Task DeleteAsync(IEnumerable<TModel> entities, bool forceDeleteEvenDeletable = false,
            CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
                await DeleteAsync(entity, forceDeleteEvenDeletable, cancellationToken);
        }

        public virtual async Task DeleteAsync(TModel entity, bool forceDeleteEvenDeletable = false,
            CancellationToken cancellationToken = default)
        {
            var deletable = entity as IDeletableEntity;

            if (deletable == null || forceDeleteEvenDeletable)
            {
                Repository.Delete(entity);
            }
            else if (!deletable.IsDeleted)
            {
                deletable.IsDeleted = true;
            }
            await SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<int> ExecuteDeleteAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            var res = await Repository.ExecuteDeleteAsync(query, cancellationToken);
            return res;
        }

        public virtual async Task<int> ExecuteUpdateAsync(IDataQuery<TModel> query,
            Expression<Func<SetPropertyCalls<TModel>, SetPropertyCalls<TModel>>> setPropertyCalls,
            CancellationToken cancellationToken = default)
        {
            return await Repository.ExecuteUpdateAsync(query, setPropertyCalls, cancellationToken);
        }

        public virtual async Task<TModel> GetAsync(TQuery query, bool tracking = false, CancellationToken cancellationToken = default)
        {
            query.Take = 1;
            return (await Repository.FetchAsync(query, tracking, cancellationToken)).FirstOrDefault();
        }

        public virtual async Task<TModel> GetByIdAsync(TKey key, bool tracking = false, IEnumerable<string> includes = null,
            CancellationToken cancellationToken = default)
        {
            var keys = new TKey[] { key };
            return (await GetByIdsAsync(keys, tracking, includes, cancellationToken)).FirstOrDefault();
        }

        public virtual async Task<List<TModel>> GetByIdsAsync(IEnumerable<TKey> keys, bool tracking = false,
            IEnumerable<string> includes = null, CancellationToken cancellationToken = default)
        {
            var query = Activator.CreateInstance<TQuery>();
            query.Ids = keys.ToArray();
            if (keys.Count() == 1)
                query.Take = 1;
            if (includes != null)
                query.Includes = includes;
            return (await Repository.FetchAsync(query, tracking)).ToList();
        }

        public virtual async Task<TModel> GetLiteAsync(TQuery query, Expression<Func<TModel, TModel>> select,
            bool tracking = false, CancellationToken cancellationToken = default)
        {
            query.Take = 1;
            return (await Repository.FetchLiteAsync(query, select, tracking)).FirstOrDefault();
        }

        public virtual async Task<TModel> GetLiteByIdAsync(TKey key, Expression<Func<TModel, TModel>> select,
            bool tracking = false, IEnumerable<string> includes = null, CancellationToken cancellationToken = default)
        {
            var keys = new TKey[] { key };
            return (await GetLiteByIdsAsync(keys, select, tracking, includes)).FirstOrDefault();
        }

        public virtual async Task<List<TModel>> GetLiteByIdsAsync(IEnumerable<TKey> keys,
            Expression<Func<TModel, TModel>> select,
            bool tracking = false, IEnumerable<string> includes = null, CancellationToken cancellationToken = default)
        {
            var query = Activator.CreateInstance<TQuery>();
            query.Ids = keys.ToArray();
            if (keys.Count() == 1)
                query.Take = 1;
            if (includes != null)
                query.Includes = includes;
            return (await Repository.FetchLiteAsync(query, select, tracking, cancellationToken)).ToList();
        }

        public virtual async Task<TModel> GetLiteAsync(TQuery query, bool tracking = false,
            CancellationToken cancellationToken = default)
        {
            return await GetLiteAsync(query, LiteSelector, tracking, cancellationToken);
        }

        public virtual async Task<TModel> GetLiteByIdAsync(TKey key, bool tracking = false,
            IEnumerable<string> includes = null, CancellationToken cancellationToken = default)
        {
            return await GetLiteByIdAsync(key, LiteSelector, tracking, includes, cancellationToken);
        }

        public virtual async Task<List<TModel>> GetLiteByIdsAsync(IEnumerable<TKey> keys, bool tracking = false,
            IEnumerable<string> includes = null, CancellationToken cancellationToken = default)
        {
            return await GetLiteByIdsAsync(keys, LiteSelector, tracking, includes, cancellationToken);
        }

        public virtual async Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default)
        {
            var query = Activator.CreateInstance<TQuery>();
            query.Ids = new[] { key };
            return await Repository.CountAsync(query, cancellationToken) > 0;
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await Repository.SaveChangesAsync(cancellationToken);
        }

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

        #endregion IDomainService implementation
    }
}
