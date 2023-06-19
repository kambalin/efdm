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
        public DatePeriodQueryParams CreatedQueryParams { get; set; } = new DatePeriodQueryParams();
        public DatePeriodQueryParams ModifiedQueryParams { get; set; } = new DatePeriodQueryParams();

        public override IQueryFilter<TModel> ToFilter() {
            var and = new QueryFilter<TModel>();

            if (CreatedByIds?.Any() == true)
                and.Add(x => CreatedByIds.Contains(x.CreatedById));

            if (ModifiedByIds?.Any() == true)
                and.Add(x => ModifiedByIds.Contains(x.ModifiedById));

            if (CreatedQueryParams.LessOrEquals.HasValue)
                and.Add(x => x.Created <= CreatedQueryParams.LessOrEquals.Value);

            if (CreatedQueryParams.MoreOrEquals.HasValue)
                and.Add(x => x.Created >= CreatedQueryParams.MoreOrEquals.Value);

            if (ModifiedQueryParams.LessOrEquals.HasValue)
                and.Add(x => x.Modified <= ModifiedQueryParams.LessOrEquals.Value);

            if (ModifiedQueryParams.MoreOrEquals.HasValue)
                and.Add(x => x.Modified >= ModifiedQueryParams.MoreOrEquals.Value);

            return base.ToFilter().Add(and);
        }
    }
}
