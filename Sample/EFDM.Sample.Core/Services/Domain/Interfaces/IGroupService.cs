using EFDM.Abstractions.Services.Domain;
using EFDM.Sample.Core.DataQueries.Models;
using EFDM.Sample.Core.Models.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace EFDM.Sample.Core.Services.Domain.Interfaces
{
    public interface IGroupService : IDomainService<Group, GroupQuery, int>
    {
        Task AddUser(int groupId, int userId, CancellationToken cancellationToken = default);
        Task RemoveUser(int groupId, int userId, CancellationToken cancellationToken = default);
    }
}
