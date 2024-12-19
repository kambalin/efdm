using EFDM.Abstractions.DAL.Repositories;
using EFDM.Core.Services.Domain;
using EFDM.Sample.Core.DataQueries.Models;
using EFDM.Sample.Core.Models.Domain;
using EFDM.Sample.Core.Services.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EFDM.Sample.Core.Services.Domain
{
    public class GroupTypeService : DomainServiceBase<GroupType, GroupTypeQuery, int, IRepository<GroupType, int>>, IGroupTypeService
    {
        public GroupTypeService(
            IRepository<GroupType, int> repository,
            ILogger logger
        ) : base(repository, logger)
        {
        }
    }
}
