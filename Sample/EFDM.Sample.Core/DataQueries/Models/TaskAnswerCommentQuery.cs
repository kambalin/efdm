using EFDM.Abstractions.DataQueries;
using EFDM.Core.DataQueries;
using EFDM.Sample.Core.Models.Domain;

namespace EFDM.Sample.Core.DataQueries.Models
{
    public class TaskAnswerCommentQuery : IdKeyDataQueryBase<TaskAnswerComment, int>
    {
        public override IQueryFilter<TaskAnswerComment> ToFilter()
        {
            var and = new QueryFilter<TaskAnswerComment>();
            return base.ToFilter().Add(and);
        }
    }
}
