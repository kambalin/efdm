using EFDM.Abstractions.DAL.Repositories;
using EFDM.Core.Services.Domain;
using EFDM.Sample.Core.DataQueries.Models;
using EFDM.Sample.Core.Models.Domain;
using EFDM.Sample.Core.Services.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EFDM.Sample.Core.Services.Domain
{
    public class TaskAnswerCommentService : DomainServiceBase<TaskAnswerComment,
        TaskAnswerCommentQuery, int, IRepository<TaskAnswerComment, int>>, ITaskAnswerCommentService
    {
        #region fields & properties

        #endregion fields & properties

        #region constructors

        public TaskAnswerCommentService(
            IRepository<TaskAnswerComment, int> repository,
            ILogger logger
        ) : base(repository, logger)
        {
        }

        #endregion constructors
    }
}
