using EFDM.Abstractions.Services.Domain;
using EFDM.Test.Core.DataQueries.Models;
using EFDM.Test.Core.Models.Domain;

namespace EFDM.Test.Core.Services.Domain.Interfaces
{
    public interface IGroupService : IDomainService<Group, GroupQuery, int>
    {
        void AddUser(int groupId, int userId);
        void RemoveUser(int groupId, int userId);
    }
}
