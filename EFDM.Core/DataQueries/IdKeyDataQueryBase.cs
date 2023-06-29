using EFDM.Abstractions.DataQueries;
using EFDM.Abstractions.Models.Domain;
using System;
using System.Linq;

namespace EFDM.Core.DataQueries
{
    public abstract class IdKeyDataQueryBase<TModel, TKey> : DataQueryBase<TModel>, IDataQuery<TModel, TKey>
        where TModel : class, IIdKeyEntity<TKey>
        where TKey : IComparable, IEquatable<TKey>
    {
        public TKey[] Ids { get; set; }
        public TKey[] NotIds { get; set; }

        public override IQueryFilter<TModel> ToFilter()
        {
            var and = new QueryFilter<TModel>();

            if (Ids?.Any() == true)
                and.Add(x => Ids.Contains(x.Id));

            if (NotIds?.Any() == true)
                and.Add(x => !NotIds.Contains(x.Id));

            return and;
        }
    }
}
