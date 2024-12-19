﻿using EFDM.Abstractions.DataQueries;
using EFDM.Abstractions.Models.Domain;
using EFDM.Abstractions.Models.Responses;
using EFDM.Abstractions.Models.Validation;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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
        IEnumerable<TModel> Fetch(TQuery query = null, bool tracking = false);
        IPagedList<TModel> FetchPaged(TQuery query = null);
        /// <summary>
        /// Fetch entities from database with only required fields
        /// </summary>
        /// <param name="query">Filter query</param>
        /// <param name="select">Fields selector</param>
        /// <param name="tracking">Track entities by the change tracker that are returned or not</param>
        /// <returns></returns>
        IEnumerable<TModel> FetchLite(TQuery query, Expression<Func<TModel, TModel>> select, bool tracking = false);
        /// <summary>
        /// Fetch entities from database with only required fields
        /// Uses default domain service fields selector (defined in service)
        /// </summary>
        /// <param name="query">Filter query</param>
        /// <param name="tracking">Track entities by the change tracker that are returned or not</param>
        /// <returns>Result entities from db</returns>
        IEnumerable<TModel> FetchLite(TQuery query, bool tracking = false);
        int Count(TQuery query = null);
        void Add(params TModel[] models);
        TModel Save(TModel model);
        void Delete(TKey id, bool forceDeleteEvenDeletable = false);
        void Delete(IEnumerable<TModel> entities, bool forceDeleteEvenDeletable = false);
        void Delete(TModel entity, bool forceDeleteEvenDeletable = false);
        int ExecuteDelete(TQuery query);
        int ExecuteUpdate(IDataQuery<TModel> query,
            Expression<Func<SetPropertyCalls<TModel>, SetPropertyCalls<TModel>>> setPropertyCalls);
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
        void ClearChangeTracker();
        /// <summary>
        /// Fetch entity id column values from table
        /// </summary>
        /// <param name="query">Query for filtering</param>
        /// <returns>List of Id values</returns>
        IEnumerable<TKey> FetchIds(IDataQuery<TModel> query);
    }
}
