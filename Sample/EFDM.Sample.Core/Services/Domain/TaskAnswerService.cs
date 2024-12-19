using EFDM.Abstractions.DAL.Repositories;
using EFDM.Core.Services.Domain;
using EFDM.Sample.Core.DataQueries.Models;
using EFDM.Sample.Core.Models.Domain;
using EFDM.Sample.Core.Services.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EFDM.Sample.Core.Services.Domain
{
    public class TaskAnswerService : DomainServiceBase<TaskAnswer,
        TaskAnswerQuery, int, IRepository<TaskAnswer, int>>, ITaskAnswerService
    {
        #region fields & properties

        #endregion fields & properties

        #region constructors

        public TaskAnswerService(
            IRepository<TaskAnswer, int> repository,
            ILogger logger
        ) : base(repository, logger)
        {

        }

        #endregion constructors
    }
}
