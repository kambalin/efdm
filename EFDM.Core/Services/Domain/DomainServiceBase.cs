using EFDM.Abstractions.DAL.Repositories;
using EFDM.Abstractions.DataQueries;
using EFDM.Abstractions.Models.Domain;
using EFDM.Abstractions.Models.Responses;
using EFDM.Abstractions.Models.Validation;
using EFDM.Abstractions.Services.Domain;
using EFDM.Core.Models.Validation;
using EFDM.Core.Services.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Core.Services.Domain {

    public abstract class DomainServiceBase<TModel, TQuery, TKey, TRepository> : ServiceBase, IDomainService<TModel, TQuery, TKey>
        where TModel : class, IIdKeyEntity<TKey>, new()
        where TKey : IComparable, IEquatable<TKey>
        where TQuery : class, IDataQuery<TModel, TKey>
        where TRepository : IRepository<TModel, TKey> {

        #region fields & properties

        protected readonly TRepository Repository;
        private Expression<Func<TModel, TModel>> _liteSelector = x => new TModel() { Id = x.Id };
        protected Expression<Func<TModel, TModel>> LiteSelector {
            get {
                if (_liteSelector == null)
                    throw new NullReferenceException($"Need to initialize '{nameof(LiteSelector)}' in domain service class");
                return _liteSelector;
            }
            set {
                _liteSelector = value;
            }
        }

        #endregion fields & properties

        #region constructors

        protected DomainServiceBase(TRepository repository, ILogger logger) : base(logger) {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #endregion constructors

        #region IDomainService implementation

        public virtual IEnumerable<TModel> Fetch(TQuery query = null, bool tracking = false) {
            var result = Repository.Fetch(query, tracking);
            return result;
        }

        public virtual IPagedList<TModel> FetchPaged(TQuery query = null) {
            var result = Repository.FetchPaged(query);
            return result;
        }

        public virtual IEnumerable<TModel> FetchLite(TQuery query, Expression<Func<TModel, TModel>> select, bool tracking = false) {
            var result = Repository.FetchLite(query, select, tracking);
            return result;
        }

        public virtual IEnumerable<TModel> FetchLite(TQuery query, bool tracking = false) {
            return FetchLite(query, LiteSelector, tracking);
        }

        public virtual int Count(TQuery query = null) {
            var result = Repository.Count(query);
            return result;
        }

        public virtual void Add(params TModel[] models) {
            Repository.Add(models);
        }

        public virtual TModel Save(TModel model) {
            var isNew = model.Id.Equals(default);
            if (isNew)
                Repository.Add(model);
            return Repository.Save(model);
        }

        public virtual void Delete(TKey id, bool forceDeleteEvenDeletable = false) {
            var entity = GetById(id, true);
            Delete(entity, forceDeleteEvenDeletable);
        }

        public virtual void Delete(IEnumerable<TModel> entities, bool forceDeleteEvenDeletable = false) {
            foreach (var entity in entities)
                Delete(entity, forceDeleteEvenDeletable);
        }

        public virtual void Delete(TModel entity, bool forceDeleteEvenDeletable = false) {
            var deletable = entity as IDeletableEntity;

            if (deletable == null || forceDeleteEvenDeletable) {
                Repository.Delete(entity);
            }
            else if (!deletable.IsDeleted) {
                deletable.IsDeleted = true;
            }
            SaveChanges();
        }

        public virtual TModel Get(TQuery query, bool tracking = false) {
            query.Take = 1;
            return Repository.Fetch(query, tracking).FirstOrDefault();
        }

        public virtual TModel GetById(TKey key, bool tracking = false, IEnumerable<string> includes = null) {
            var keys = new TKey[] { key };
            return GetByIds(keys, tracking, includes).FirstOrDefault();
        }

        public virtual List<TModel> GetByIds(IEnumerable<TKey> keys, bool tracking = false, IEnumerable<string> includes = null) {
            var query = Activator.CreateInstance<TQuery>();
            query.Ids = keys.ToArray();
            if (keys.Count() == 1)
                query.Take = 1;
            if (includes != null)
                query.Includes = includes;
            return Repository.Fetch(query, tracking).ToList();
        }

        public virtual TModel GetLite(TQuery query, Expression<Func<TModel, TModel>> select, bool tracking = false) {
            query.Take = 1;
            return Repository.FetchLite(query, select, tracking).FirstOrDefault();
        }

        public virtual TModel GetLiteById(TKey key, Expression<Func<TModel, TModel>> select, bool tracking = false, IEnumerable<string> includes = null) {
            var keys = new TKey[] { key };
            return GetLiteByIds(keys, select, tracking, includes).FirstOrDefault();
        }

        public virtual List<TModel> GetLiteByIds(IEnumerable<TKey> keys, Expression<Func<TModel, TModel>> select, bool tracking = false, IEnumerable<string> includes = null) {
            var query = Activator.CreateInstance<TQuery>();
            query.Ids = keys.ToArray();
            if (keys.Count() == 1)
                query.Take = 1;
            if (includes != null)
                query.Includes = includes;
            return Repository.FetchLite(query, select, tracking).ToList();
        }

        public virtual TModel GetLite(TQuery query, bool tracking = false) {
            return GetLite(query, LiteSelector, tracking);
        }

        public virtual TModel GetLiteById(TKey key, bool tracking = false, IEnumerable<string> includes = null) {
            return GetLiteById(key, LiteSelector, tracking, includes);
        }

        public virtual List<TModel> GetLiteByIds(IEnumerable<TKey> keys, bool tracking = false, IEnumerable<string> includes = null) {
            return GetLiteByIds(keys, LiteSelector, tracking, includes);
        }

        public virtual bool Exists(TKey key) {
            var query = Activator.CreateInstance<TQuery>();
            query.Ids = new[] { key };
            return Repository.Count(query) > 0;
        }

        public virtual int SaveChanges() {
            return Repository.SaveChanges();
        }

        public virtual IValidationResult DeleteValidation(TModel model) {
            return new ValidationResult();
        }

        public virtual bool IsCreator(TModel model, int userId) {
            IAuditableEntity auditable = model as IAuditableEntity;
            if (model != null)
                return auditable.CreatedById == userId;
            return false;
        }

        public virtual void ResetContextState() {
            Repository.ResetContextState();
        }

        #endregion IDomainService implementation
    }
}
