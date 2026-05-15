using EFDM.Abstractions.DataQueries;
using EFDM.Abstractions.Models.Domain;
using LinqKit;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EFDM.Core.DataQueries
{
    public class EntityDataQueryBase<TModel, TKey> : IdKeyDataQueryBase<TModel, TKey>
        where TModel : class, IEntityBase<TKey>
        where TKey : IComparable, IEquatable<TKey>
    {
        public int[] CreatedByIds { get; set; }
        public int[] ModifiedByIds { get; set; }
        public DateOffsetPeriodQueryParams CreatedQueryParams { get; set; }
        public DateOffsetPeriodQueryParams ModifiedQueryParams { get; set; }

        public override IQueryFilter<TModel> ToFilter()
        {
            var and = new QueryFilter<TModel>();

            if (CreatedByIds?.Any() == true)
                and.Add(x => CreatedByIds.Contains(x.CreatedById));

            if (ModifiedByIds?.Any() == true)
                and.Add(x => ModifiedByIds.Contains(x.ModifiedById));


            if (CreatedQueryParams != null)
            {
                var createdQueryParamsCondition = false;
                Expression<Func<TModel, bool>> predicate = PredicateBuilder.New<TModel>(true);

                if (CreatedQueryParams.LessOrEquals.HasValue)
                {
                    predicate = predicate.And(x => x.Created <= CreatedQueryParams.LessOrEquals.Value);
                    createdQueryParamsCondition = true;
                }
                if (CreatedQueryParams.MoreOrEquals.HasValue)
                {
                    predicate = predicate.And(x => x.Created >= CreatedQueryParams.MoreOrEquals.Value);
                    createdQueryParamsCondition = true;
                }
                if (CreatedQueryParams.OrIsNull.HasValue && CreatedQueryParams.OrIsNull.Value == true)
                {
                    predicate = predicate.Or(x => x.Created.Equals(null));
                    createdQueryParamsCondition = true;
                }

                if (createdQueryParamsCondition)
                {
                    and.Add(x => predicate.Invoke(x));
                }
            }

            if (ModifiedQueryParams != null)
            {
                var modifiedQueryParamsCondition = false;
                Expression<Func<TModel, bool>> predicate = PredicateBuilder.New<TModel>(true);

                if (ModifiedQueryParams.LessOrEquals.HasValue)
                {
                    predicate = predicate.And(x => x.Modified <= ModifiedQueryParams.LessOrEquals.Value);
                    modifiedQueryParamsCondition = true;
                }
                if (ModifiedQueryParams.MoreOrEquals.HasValue)
                {
                    predicate = predicate.And(x => x.Modified >= ModifiedQueryParams.MoreOrEquals.Value);
                    modifiedQueryParamsCondition = true;
                }
                if (ModifiedQueryParams.OrIsNull.HasValue && ModifiedQueryParams.OrIsNull.Value == true)
                {
                    predicate = predicate.Or(x => x.Modified.Equals(null));
                    modifiedQueryParamsCondition = true;
                }

                if (modifiedQueryParamsCondition)
                {
                    and.Add(x => predicate.Invoke(x));
                }
            }

            return base.ToFilter().Add(and);
        }
    }
}
