using EFDM.Abstractions.DataQueries;
using EFDM.Core.DataQueries;
using EFDM.Test.Core.Models.Domain;
using System;
using System.Linq;

namespace EFDM.Test.Core.DataQueries.Models {

    public class TaskAnswerCommentQuery : IdKeyDataQueryBase<TaskAnswerComment, int> {

        public override IQueryFilter<TaskAnswerComment> ToFilter() {
            var and = new QueryFilter<TaskAnswerComment>();
            return base.ToFilter().Add(and);
        }
    }
}
