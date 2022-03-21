using EFDM.Abstractions.DataQueries;
using EFDM.Abstractions.Models.Domain;
using EFDM.Abstractions.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EFDM.Abstractions.DAL.Repositories {

    public interface IRepository<TEntity, TKey>
        where TEntity : IIdKeyEntity<TKey>
        where TKey : IComparable, IEquatable<TKey> {

        bool AutoDetectChanges { get; set; }
        IEnumerable<TEntity> Fetch(IDataQuery<TEntity> query = null, bool tracking = false);
        IPagedList<TEntity> FetchPaged(IDataQuery<TEntity> query = null, bool tracking = false);
        IEnumerable<TEntity> FetchLite(IDataQuery<TEntity> query, Expression<Func<TEntity, TEntity>> select, bool tracking = false);
        int Count(IDataQuery<TEntity> query = null);
        void Add(params TEntity[] entities);
        void Update(params TEntity[] entities);
        void Delete(params TEntity[] entities);
        TEntity Save(TEntity entity);
        TEntity Save(TEntity model, params Expression<Func<TEntity, object>>[] updateProperties);
        int SaveChanges();
        IQueryable<TEntity> Queryable();
        IQueryable<TEntity> QueryableSql(string sql, params object[] parameters);
        bool IsAttached(TKey id);
        void ResetContextState();
    }
}
