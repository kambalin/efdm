using EFCore.BulkExtensions;
using EFDM.Abstractions.DataQueries;
using EFDM.Abstractions.Models.Domain;
using EFDM.Abstractions.Models.Responses;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
        IEnumerable<TEntity> Fetch(IDataQuery<TEntity> query = null, bool tracking = false);
        IPagedList<TEntity> FetchPaged(IDataQuery<TEntity> query = null, bool tracking = false);
        IEnumerable<TEntity> FetchLite(IDataQuery<TEntity> query, Expression<Func<TEntity, TEntity>> select, bool tracking = false);
        int Count(IDataQuery<TEntity> query = null);
        void Add(params TEntity[] entities);
        void Update(params TEntity[] entities);
        void Delete(params TEntity[] entities);
        /// <summary>
        /// Deletes all database rows for the entity instances which match the query. 
        /// Executes immediately and does not interact with the EF change tracker (see efcore bulk operations).
        /// </summary>
        /// <param name="query">Query for entities.</param>
        /// <returns>The total number of rows deleted in the database.</returns>
        int ExecuteDelete(IDataQuery<TEntity> query);
        /// <summary>
        /// Updates all database rows for the entity instances which match the query from the database
        /// Executes immediately and does not interact with the EF change tracker (see efcore bulk operations).
        /// </summary>
        /// <param name="query">Query for entities.</param>
        /// <param name="setPropertyCalls">A collection of set property statements specifying properties to update.</param>
        /// <returns>The total number of rows updated in the database.</returns>
        int ExecuteUpdate(IDataQuery<TEntity> query,
            Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls);
        /// <summary>
        /// Save entity, even new, detached or not trackable
        /// </summary>
        /// <param name="entity">Entity to save</param>
        /// <returns></returns>
        TEntity Save(TEntity entity);
        /// <summary>
        /// Save entity with only specified properties
        /// </summary>
        /// <param name="model">Entity to save</param>
        /// <param name="updateProperties">Properties that must be updated</param>
        /// <returns></returns>
        TEntity Save(TEntity model, params Expression<Func<TEntity, object>>[] updateProperties);
        int SaveChanges();
        IQueryable<TEntity> Queryable();
        IQueryable<TEntity> QueryableSql(string sql, params object[] parameters);
        bool IsAttached(TKey id);
        void ClearChangeTracker();
        void BulkInsert(IList<TEntity> entities, BulkConfig config);
        void BulkInsertOrUpdate(IList<TEntity> entities, BulkConfig config);
        /// <summary>
        /// Fetch entity id column values from table
        /// </summary>
        /// <param name="query">Query for filtering</param>
        /// <returns>List of Id values</returns>
        IEnumerable<TKey> FetchIds(IDataQuery<TEntity> query);
    }
}
