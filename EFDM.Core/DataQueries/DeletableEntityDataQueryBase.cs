﻿using EFDM.Abstractions.DataQueries;
using EFDM.Abstractions.Models.Domain;
using System;

namespace EFDM.Core.DataQueries
{
    public class DeletableEntityDataQueryBase<TModel, TKey> : EntityDataQueryBase<TModel, TKey>
        where TModel : class, IDeletableEntity, IEntityBase<TKey>
        where TKey : IComparable, IEquatable<TKey>
    {
        public bool? IsDeleted { get; set; }

        public override IQueryFilter<TModel> ToFilter()
        {
            var and = new QueryFilter<TModel>();

            if (IsDeleted != null)
                and.Add(x => x.IsDeleted == IsDeleted);

            return base.ToFilter().Add(and);
        }
    }
}
