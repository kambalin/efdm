using EFDM.Abstractions.DAL.Repositories;
using EFDM.Core.Services.Domain;
using EFDM.Test.Core.DataQueries.Models;
using EFDM.Test.Core.Models.Domain;
using EFDM.Test.Core.Services.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EFDM.Test.Core.Services.Domain {

    public class TaskAnswerCommentService : DomainServiceBase<TaskAnswerComment,
        TaskAnswerCommentQuery, int, IRepository<TaskAnswerComment, int>>, ITaskAnswerCommentService {

        #region fields & properties

        #endregion fields & properties

        #region constructors

        public TaskAnswerCommentService(
            IRepository<TaskAnswerComment, int> repository,
            ILogger logger
        ) : base(repository, logger) {

        }

        #endregion constructors
    }
}
