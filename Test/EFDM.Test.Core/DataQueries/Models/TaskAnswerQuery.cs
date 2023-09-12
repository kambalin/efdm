using EFDM.Abstractions.DataQueries;
using EFDM.Core.DataQueries;
using EFDM.Test.Core.Models.Domain;
using LinqKit;

namespace EFDM.Test.Core.DataQueries.Models
{
    public class TaskAnswerQuery : IdKeyDataQueryBase<TaskAnswer, int>
    {
        public DatePeriodQueryParams? ValidFromQueryParams { get; set; }
        public DateOffsetPeriodQueryParams? ValidFromOffsetQueryParams { get; set; }

        public override IQueryFilter<TaskAnswer> ToFilter()
        {
            var and = new QueryFilter<TaskAnswer>();

            if (ValidFromQueryParams != null)
            {
                var validFromQueryParamsCondition = false;
                var predicate = PredicateBuilder.True<TaskAnswer>();

                if (ValidFromQueryParams.LessOrEquals.HasValue)
                {
                    predicate = predicate.And(x => x.ValidFrom <= ValidFromQueryParams.LessOrEquals.Value);
                    validFromQueryParamsCondition = true;
                }
                if (ValidFromQueryParams.MoreOrEquals.HasValue)
                {
                    predicate = predicate.And(x => x.ValidFrom >= ValidFromQueryParams.MoreOrEquals.Value);
                    validFromQueryParamsCondition = true;
                }
                if (ValidFromQueryParams.OrIsNull.HasValue && ValidFromQueryParams.OrIsNull.Value == true)
                {
                    predicate = predicate.Or(x => x.ValidFrom.Equals(null));
                    validFromQueryParamsCondition = true;
                }

                if (validFromQueryParamsCondition)
                {
                    and.Add(x => predicate.Invoke(x));
                }
            }

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
                {
                    and.Add(x => predicate.Invoke(x));
                }
            }

            return base.ToFilter().Add(and);
        }
    }
}
