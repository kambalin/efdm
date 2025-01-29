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
        Task<IPagedList<TModel>> FetchPagedAsync(TQuery query = null, CancellationToken cancellationToken = default);
        /// <summary>
        /// Fetch entities from database with only required fields
        /// </summary>
        /// <param name="query">Filter query</param>
        /// <param name="select">Fields selector</param>
        /// <param name="tracking">Track entities by the change tracker that are returned or not</param>
        /// <returns></returns>
        Task<IEnumerable<TModel>> FetchLiteAsync(TQuery query, Expression<Func<TModel, TModel>> select,
            bool tracking = false, CancellationToken cancellationToken = default);
        /// <summary>
        /// Fetch entities from database with only required fields
        /// Uses default domain service fields selector (defined in service)
        /// </summary>
        /// <param name="query">Filter query</param>
        /// <param name="tracking">Track entities by the change tracker that are returned or not</param>
        /// <returns>Result entities from db</returns>
        Task<IEnumerable<TModel>> FetchLiteAsync(TQuery query, bool tracking = false, CancellationToken cancellationToken = default);
        Task<int> CountAsync(TQuery query = null, CancellationToken cancellationToken = default);
        Task AddAsync(params TModel[] models);
        Task<TModel> SaveAsync(TModel model, CancellationToken cancellationToken = default);
        Task DeleteAsync(TKey id, bool forceDeleteEvenDeletable = false, CancellationToken cancellationToken = default);
        Task DeleteAsync(IEnumerable<TModel> entities, bool forceDeleteEvenDeletable = false, CancellationToken cancellationToken = default);
        Task DeleteAsync(TModel entity, bool forceDeleteEvenDeletable = false, CancellationToken cancellationToken = default);
        Task<int> ExecuteDeleteAsync(TQuery query, CancellationToken cancellationToken = default);
        Task<int> ExecuteUpdateAsync(IDataQuery<TModel> query,
            Expression<Func<SetPropertyCalls<TModel>, SetPropertyCalls<TModel>>> setPropertyCalls,
            CancellationToken cancellationToken = default);
        Task<TModel> GetAsync(TQuery query, bool tracking = false, CancellationToken cancellationToken = default);
        Task<TModel> GetByIdAsync(TKey key, bool tracking = false, IEnumerable<string> includes = null,
            CancellationToken cancellationToken = default);
        Task<List<TModel>> GetByIdsAsync(IEnumerable<TKey> keys, bool tracking = false,
            IEnumerable<string> includes = null, CancellationToken cancellationToken = default);
        Task<TModel> GetLiteAsync(TQuery query, Expression<Func<TModel, TModel>> select, bool tracking = false,
            CancellationToken cancellationToken = default);
        Task<TModel> GetLiteByIdAsync(TKey key, Expression<Func<TModel, TModel>> select, bool tracking = false,
            IEnumerable<string> includes = null, CancellationToken cancellationToken = default);
        Task<List<TModel>> GetLiteByIdsAsync(IEnumerable<TKey> keys, Expression<Func<TModel, TModel>> select,
            bool tracking = false, IEnumerable<string> includes = null, CancellationToken cancellationToken = default);
        Task<TModel> GetLiteAsync(TQuery query, bool tracking = false, CancellationToken cancellationToken = default);
        Task<TModel> GetLiteByIdAsync(TKey key, bool tracking = false, IEnumerable<string> includes = null,
            CancellationToken cancellationToken = default);
        Task<List<TModel>> GetLiteByIdsAsync(IEnumerable<TKey> keys, bool tracking = false,
            IEnumerable<string> includes = null, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        IValidationResult DeleteValidation(TModel model);
        bool IsCreator(TModel model, int userId);
        void ClearChangeTracker();
        /// <summary>
        /// Fetch entity id column values from table
        /// </summary>
        /// <param name="query">Query for filtering</param>
        /// <returns>List of Id values</returns>
        Task<IEnumerable<TKey>> FetchIdsAsync(IDataQuery<TModel> query, CancellationToken cancellationToken = default);
    }
}
