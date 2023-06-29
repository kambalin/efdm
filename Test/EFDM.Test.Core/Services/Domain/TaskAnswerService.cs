using EFDM.Abstractions.DAL.Repositories;
using EFDM.Core.Services.Domain;
using EFDM.Test.Core.DataQueries.Models;
using EFDM.Test.Core.Models.Domain;
using EFDM.Test.Core.Services.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EFDM.Test.Core.Services.Domain
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
