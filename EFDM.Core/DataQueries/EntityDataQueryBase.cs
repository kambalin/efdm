using EFDM.Abstractions.DataQueries;
using EFDM.Abstractions.Models.Domain;
using System;
using System.Linq;

namespace EFDM.Core.DataQueries {

    public class EntityDataQueryBase<TModel, TKey> : IdKeyDataQueryBase<TModel, TKey>
        where TModel : class, IEntityBase<TKey>
        where TKey : IComparable, IEquatable<TKey> {

        public int[] CreatedByIds { get; set; }
        public int[] ModifiedByIds { get; set; }
        public DateTimeOffset? FromModifiedBy { get; set; }

        public override IQueryFilter<TModel> ToFilter() {
            var and = new QueryFilter<TModel>();

            if (CreatedByIds?.Any() == true)
                and.Add(x => CreatedByIds.Contains(x.CreatedById));

            if (ModifiedByIds?.Any() == true)
                and.Add(x => ModifiedByIds.Contains(x.ModifiedById));

            if (FromModifiedBy.HasValue)
                and.Add(x => x.Modified >= FromModifiedBy.Value);

            return base.ToFilter().Add(and);
        }
    }
}
