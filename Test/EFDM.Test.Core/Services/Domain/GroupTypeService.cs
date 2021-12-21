using EFDM.Abstractions.DAL.Repositories;
using EFDM.Core.Services.Domain;
using EFDM.Test.Core.DataQueries.Models;
using EFDM.Test.Core.Models.Domain;
using EFDM.Test.Core.Services.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Test.Core.Services.Domain {

    public class GroupTypeService : DomainServiceBase<GroupType, GroupTypeQuery, int, IRepository<GroupType, int>>, IGroupTypeService {

        public GroupTypeService(
            IRepository<GroupType, int> repository,
            ILogger logger
        ) : base(repository, logger) {

        }
    }
}
