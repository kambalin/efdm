using EFDM.Abstractions.Services.Domain;
using EFDM.Test.Core.DataQueries.Models;
using EFDM.Test.Core.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Test.Core.Services.Domain.Interfaces {

    public interface IGroupService : IDomainService<Group, GroupQuery, int> {
        void AddUser(int groupId, int userId);
        void RemoveUser(int groupId, int userId);
    }
}
