using EFDM.Abstractions.Services.Domain;
using EFDM.Sample.Core.DataQueries.Models;
using EFDM.Sample.Core.Models.Domain;

namespace EFDM.Sample.Core.Services.Domain.Interfaces
{
    public interface IGroupService : IDomainService<Group, GroupQuery, int>
    {
        void AddUser(int groupId, int userId);
        void RemoveUser(int groupId, int userId);
    }
}
