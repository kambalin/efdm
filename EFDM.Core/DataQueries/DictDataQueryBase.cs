﻿using EFDM.Abstractions.DataQueries;
using EFDM.Abstractions.Models.Domain;
using System;

namespace EFDM.Core.DataQueries
{
    public class DictDataQueryBase<TModel, TKey> : EntityDataQueryBase<TModel, TKey>
        where TModel : class, IDictEntityBase<TKey>
        where TKey : IComparable, IEquatable<TKey>
    {
        public string Title { get; set; }

        public override IQueryFilter<TModel> ToFilter()
        {
            var and = new QueryFilter<TModel>();

            if (!string.IsNullOrEmpty(Title))
            {
                foreach (var word in GetWords(Title))
                {
                    and.Add(x => x.Title.Contains(word));
                }
            }

            return base.ToFilter().Add(and);
        }
    }
}
