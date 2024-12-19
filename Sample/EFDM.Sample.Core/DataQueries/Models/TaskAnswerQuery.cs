using EFDM.Abstractions.DataQueries;
using EFDM.Core.DataQueries;
using EFDM.Sample.Core.Models.Domain;
using LinqKit;

namespace EFDM.Sample.Core.DataQueries.Models
{
    public class TaskAnswerQuery : IdKeyDataQueryBase<TaskAnswer, int>
    {
        public DateOffsetPeriodQueryParams? ValidFromOffsetQueryParams { get; set; }
        public DateOffsetPeriodQueryParams? ValidTillOffsetQueryParams { get; set; }

        public override IQueryFilter<TaskAnswer> ToFilter()
        {
            var and = new QueryFilter<TaskAnswer>();

            if (ValidFromOffsetQueryParams != null)
            {
                var validFromOffsetQueryParamsCondition = false;
                var predicate = PredicateBuilder.True<TaskAnswer>();

                if (ValidFromOffsetQueryParams.LessOrEquals.HasValue)
                {
                    predicate = predicate.And(x => x.ValidFrom <= ValidFromOffsetQueryParams.LessOrEquals.Value);
                    validFromOffsetQueryParamsCondition = true;
                }
                if (ValidFromOffsetQueryParams.MoreOrEquals.HasValue)
                {
                    predicate = predicate.And(x => x.ValidFrom >= ValidFromOffsetQueryParams.MoreOrEquals.Value);
                    validFromOffsetQueryParamsCondition = true;
                }
                if (ValidFromOffsetQueryParams.OrIsNull.HasValue && ValidFromOffsetQueryParams.OrIsNull.Value == true)
                {
                    predicate = predicate.Or(x => x.ValidFrom.Equals(null));
                    validFromOffsetQueryParamsCondition = true;
                }

                if (validFromOffsetQueryParamsCondition)
                    and.Add(x => predicate.Invoke(x));
            }

            if (ValidTillOffsetQueryParams != null)
            {
                var validTillOffsetQueryParamsCondition = false;
                var predicate = PredicateBuilder.True<TaskAnswer>();

                if (ValidTillOffsetQueryParams.LessOrEquals.HasValue)
                {
                    predicate = predicate.And(x => x.ValidTill <= ValidTillOffsetQueryParams.LessOrEquals.Value);
                    validTillOffsetQueryParamsCondition = true;
                }
                if (ValidTillOffsetQueryParams.MoreOrEquals.HasValue)
                {
                    predicate = predicate.And(x => x.ValidTill >= ValidTillOffsetQueryParams.MoreOrEquals.Value);
                    validTillOffsetQueryParamsCondition = true;
                }
                if (ValidTillOffsetQueryParams.OrIsNull.HasValue && ValidTillOffsetQueryParams.OrIsNull.Value == true)
                {
                    predicate = predicate.Or(x => x.ValidTill.Equals(null));
                    validTillOffsetQueryParamsCondition = true;
                }

                if (validTillOffsetQueryParamsCondition)
                    and.Add(x => predicate.Invoke(x));
            }

            return base.ToFilter().Add(and);
        }
    }
}
