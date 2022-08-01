using EFDM.Abstractions.DataQueries;
using EFDM.Abstractions.Models.Domain;
using EFDM.Abstractions.Models.Responses;
using EFDM.Abstractions.Models.Validation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EFDM.Abstractions.Services.Domain {

    public interface IDomainService<TModel, TQuery, TKey>
        where TModel : class, IIdKeyEntity<TKey>
        where TKey : IComparable, IEquatable<TKey>
        where TQuery : class, IDataQuery<TModel, TKey> {

        int ExecutorId { get; }
        IEnumerable<TModel> Fetch(TQuery query = null, bool tracking = false);
        IPagedList<TModel> FetchPaged(TQuery query = null);
        IEnumerable<TModel> FetchLite(TQuery query, Expression<Func<TModel, TModel>> select, bool tracking = false);
        IEnumerable<TModel> FetchLite(TQuery query, bool tracking = false);
        int Count(TQuery query = null);
        void Add(params TModel[] models);
        TModel Save(TModel model);
        void Delete(TKey id, bool forceDeleteEvenDeletable = false);
        void Delete(IEnumerable<TModel> entities, bool forceDeleteEvenDeletable = false);
        void Delete(TModel entity, bool forceDeleteEvenDeletable = false);
        TModel Get(TQuery query, bool tracking = false);
        TModel GetById(TKey key, bool tracking = false, IEnumerable<string> includes = null);
        List<TModel> GetByIds(IEnumerable<TKey> keys, bool tracking = false, IEnumerable<string> includes = null);
        TModel GetLite(TQuery query, Expression<Func<TModel, TModel>> select, bool tracking = false);
        TModel GetLiteById(TKey key, Expression<Func<TModel, TModel>> select, bool tracking = false, IEnumerable<string> includes = null);
        List<TModel> GetLiteByIds(IEnumerable<TKey> keys, Expression<Func<TModel, TModel>> select, bool tracking = false, IEnumerable<string> includes = null);
        TModel GetLite(TQuery query, bool tracking = false);
        TModel GetLiteById(TKey key, bool tracking = false, IEnumerable<string> includes = null);
        List<TModel> GetLiteByIds(IEnumerable<TKey> keys, bool tracking = false, IEnumerable<string> includes = null);
        bool Exists(TKey key);
        int SaveChanges();
        IValidationResult DeleteValidation(TModel model);
        bool IsCreator(TModel model, int userId);
        void ResetContextState();
    }
}
