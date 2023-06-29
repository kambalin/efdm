using EFDM.Abstractions.DataQueries;
using EFDM.Core.DataQueries;
using EFDM.Test.Core.Models.Domain;

namespace EFDM.Test.Core.DataQueries.Models
{
    public class TaskAnswerQuery : IdKeyDataQueryBase<TaskAnswer, int>
    {
        public override IQueryFilter<TaskAnswer> ToFilter()
        {
            var and = new QueryFilter<TaskAnswer>();
            return base.ToFilter().Add(and);
        }
    }
}
