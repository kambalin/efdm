using EFDM.Abstractions.DataQueries;
using EFDM.Abstractions.Models.Domain;
using EFDM.Abstractions.Models.Responses;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EFDM.Abstractions.DAL.Repositories
{
    public interface IRepository<TEntity, TKey>
        where TEntity : IIdKeyEntity<TKey>, new()
        where TKey : IComparable, IEquatable<TKey>
    {
        bool AutoDetectChanges { get; set; }
        /// <summary>
        /// Globally affected to DbContext
        /// </summary>
        public bool AutoDetectChangesEnabled { get; set; }
        int ExecutorId { get; }
        Task<IEnumerable<TEntity>> FetchAsync(IDataQuery<TEntity> query = null, bool tracking = false,
            CancellationToken cancellationToken = default);
        Task<IPagedList<TEntity>> FetchPagedAsync(IDataQuery<TEntity> query = null, bool tracking = false,
            CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FetchLiteAsync(IDataQuery<TEntity> query, Expression<Func<TEntity, TEntity>> select,
            bool tracking = false, CancellationToken cancellationToken = default);
        Task<int> CountAsync(IDataQuery<TEntity> query = null, CancellationToken cancellationToken = default);
        Task AddAsync(params TEntity[] entities);
        void Update(params TEntity[] entities);
        void Delete(params TEntity[] entities);
        /// <summary>
        /// Deletes all database rows for the entity instances which match the query. 
        /// Executes immediately and does not interact with the EF change tracker (see efcore bulk operations).
        /// </summary>
        /// <param name="query">Query for entities.</param>
        /// <returns>The total number of rows deleted in the database.</returns>
        Task<int> ExecuteDeleteAsync(IDataQuery<TEntity> query, CancellationToken cancellationToken = default);
        /// <summary>
        /// Updates all database rows for the entity instances which match the query from the database
        /// Executes immediately and does not interact with the EF change tracker (see efcore bulk operations).
        /// </summary>
        /// <param name="query">Query for entities.</param>
        /// <param name="setPropertyCalls">A collection of set property statements specifying properties to update.</param>
        /// <returns>The total number of rows updated in the database.</returns>
        Task<int> ExecuteUpdateAsync(IDataQuery<TEntity> query,
            Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls,
            CancellationToken cancellationToken = default);
        /// <summary>
        /// Save entity, even new, detached or not trackable
        /// </summary>
        /// <param name="entity">Entity to save</param>
        /// <returns></returns>
        Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken = default);
        /// <summary>
        /// Save entity with only specified properties
        /// </summary>
        /// <param name="model">Entity to save</param>
        /// <param name="updateProperties">Properties that must be updated</param>
        /// <returns></returns>
        Task<TEntity> SaveAsync(TEntity model, CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] updateProperties);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        IQueryable<TEntity> QueryableSql(string sql, params object[] parameters);
        bool IsAttached(TKey id);
        void ClearChangeTracker();
        /// <summary>
        /// Fetch entity id column values from table
        /// </summary>
        /// <param name="query">Query for filtering</param>
        /// <returns>List of Id values</returns>
        Task<IEnumerable<TKey>> FetchIdsAsync(IDataQuery<TEntity> query, CancellationToken cancellationToken = default);
    }
}
