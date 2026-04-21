using EFDM.Abstractions.DataQueries;
using EFDM.Abstractions.Models.Domain;
using EFDM.Abstractions.Models.Responses;
using EFDM.Abstractions.Models.Validation;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EFDM.Abstractions.Services.Domain
{
    public interface IDomainService<TModel, TQuery, TKey>
        where TModel : class, IIdKeyEntity<TKey>
        where TKey : IComparable, IEquatable<TKey>
        where TQuery : class, IDataQuery<TModel, TKey>
    {
        int ExecutorId { get; }
        /// <summary>
        /// Globally affected to DbContext
        /// </summary>
        public bool AutoDetectChangesEnabled { get; set; }
        Task<IEnumerable<TModel>> FetchAsync(TQuery query = null, bool tracking = false,
            CancellationToken cancellationToken = default);
        IEnumerable<TModel> Fetch(TQuery query = null, bool tracking = false);
        Task<IPagedList<TModel>> FetchPagedAsync(TQuery query = null, CancellationToken cancellationToken = default);
        IPagedList<TModel> FetchPaged(TQuery query = null);
        /// <summary>
        /// Fetch entities from database with only required fields
        /// </summary>
        /// <param name="query">Filter query</param>
        /// <param name="select">Fields selector</param>
        /// <param name="tracking">Track entities by the change tracker that are returned or not</param>
        /// <returns></returns>
        Task<IEnumerable<TModel>> FetchLiteAsync(TQuery query, Expression<Func<TModel, TModel>> select,
            bool tracking = false, CancellationToken cancellationToken = default);
        IEnumerable<TModel> FetchLite(TQuery query, Expression<Func<TModel, TModel>> select, bool tracking = false);
        /// <summary>
        /// Fetch entities from database with only required fields
        /// Uses default domain service fields selector (defined in service)
        /// </summary>
        /// <param name="query">Filter query</param>
        /// <param name="tracking">Track entities by the change tracker that are returned or not</param>
        /// <returns>Result entities from db</returns>
        Task<IEnumerable<TModel>> FetchLiteAsync(TQuery query, bool tracking = false, CancellationToken cancellationToken = default);
        IEnumerable<TModel> FetchLite(TQuery query, bool tracking = false);
        Task<int> CountAsync(TQuery query = null, CancellationToken cancellationToken = default);
        int Count(TQuery query = null);
        Task AddAsync(params TModel[] models);
        void Add(params TModel[] models);
        Task<TModel> SaveAsync(TModel model, CancellationToken cancellationToken = default);
        TModel Save(TModel model);
        Task DeleteAsync(TKey id, bool forceDeleteEvenDeletable = false, CancellationToken cancellationToken = default);
        void Delete(TKey id, bool forceDeleteEvenDeletable = false);
        Task DeleteAsync(IEnumerable<TModel> entities, bool forceDeleteEvenDeletable = false, CancellationToken cancellationToken = default);
        void Delete(IEnumerable<TModel> entities, bool forceDeleteEvenDeletable = false);
        Task DeleteAsync(TModel entity, bool forceDeleteEvenDeletable = false, CancellationToken cancellationToken = default);
        void Delete(TModel entity, bool forceDeleteEvenDeletable = false);
        Task<int> ExecuteDeleteAsync(TQuery query, CancellationToken cancellationToken = default);
        int ExecuteDelete(TQuery query);
        Task<int> ExecuteUpdateAsync(IDataQuery<TModel> query,
            Expression<Func<SetPropertyCalls<TModel>, SetPropertyCalls<TModel>>> setPropertyCalls,
            CancellationToken cancellationToken = default);
        int ExecuteUpdate(IDataQuery<TModel> query,
            Expression<Func<SetPropertyCalls<TModel>, SetPropertyCalls<TModel>>> setPropertyCalls);
        Task<TModel> GetAsync(TQuery query, bool tracking = false, CancellationToken cancellationToken = default);
        TModel Get(TQuery query, bool tracking = false);
        Task<TModel> GetByIdAsync(TKey key, bool tracking = false, IEnumerable<string> includes = null,
            CancellationToken cancellationToken = default);
        TModel GetById(TKey key, bool tracking = false, IEnumerable<string> includes = null);
        Task<List<TModel>> GetByIdsAsync(IEnumerable<TKey> keys, bool tracking = false,
            IEnumerable<string> includes = null, CancellationToken cancellationToken = default);
        List<TModel> GetByIds(IEnumerable<TKey> keys, bool tracking = false, IEnumerable<string> includes = null);
        Task<TModel> GetLiteAsync(TQuery query, Expression<Func<TModel, TModel>> select, bool tracking = false,
            CancellationToken cancellationToken = default);
        TModel GetLite(TQuery query, Expression<Func<TModel, TModel>> select, bool tracking = false);
        Task<TModel> GetLiteByIdAsync(TKey key, Expression<Func<TModel, TModel>> select, bool tracking = false,
            IEnumerable<string> includes = null, CancellationToken cancellationToken = default);
        TModel GetLiteById(TKey key, Expression<Func<TModel, TModel>> select, bool tracking = false,
            IEnumerable<string> includes = null);
        Task<List<TModel>> GetLiteByIdsAsync(IEnumerable<TKey> keys, Expression<Func<TModel, TModel>> select,
            bool tracking = false, IEnumerable<string> includes = null, CancellationToken cancellationToken = default);
        List<TModel> GetLiteByIds(IEnumerable<TKey> keys, Expression<Func<TModel, TModel>> select,
            bool tracking = false, IEnumerable<string> includes = null);
        Task<TModel> GetLiteAsync(TQuery query, bool tracking = false, CancellationToken cancellationToken = default);
        TModel GetLite(TQuery query, bool tracking = false);
        Task<TModel> GetLiteByIdAsync(TKey key, bool tracking = false, IEnumerable<string> includes = null,
            CancellationToken cancellationToken = default);
        TModel GetLiteById(TKey key, bool tracking = false, IEnumerable<string> includes = null);
        Task<List<TModel>> GetLiteByIdsAsync(IEnumerable<TKey> keys, bool tracking = false,
            IEnumerable<string> includes = null, CancellationToken cancellationToken = default);
        List<TModel> GetLiteByIds(IEnumerable<TKey> keys, bool tracking = false, IEnumerable<string> includes = null);
        Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default);
        bool Exists(TKey key);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
        IValidationResult DeleteValidation(TModel model);
        bool IsCreator(TModel model, int userId);
        void ClearChangeTracker();
        /// <summary>
        /// Fetch entity id column values from table
        /// </summary>
        /// <param name="query">Query for filtering</param>
        /// <returns>List of Id values</returns>
        Task<IEnumerable<TKey>> FetchIdsAsync(IDataQuery<TModel> query, CancellationToken cancellationToken = default);
        IEnumerable<TKey> FetchIds(IDataQuery<TModel> query);
    }
}
