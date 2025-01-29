using EFDM.Abstractions.DAL.Repositories;
using EFDM.Core.Services.Domain;
using EFDM.Sample.Core.DataQueries.Models;
using EFDM.Sample.Core.Models.Domain;
using EFDM.Sample.Core.Services.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EFDM.Sample.Core.Services.Domain
{
    public class GroupService : DomainServiceBase<Group, GroupQuery, int, IRepository<Group, int>>, IGroupService
    {
        readonly IRepository<User, int> UserRepo;

        public GroupService(
            IRepository<User, int> userRepo,
            IRepository<Group, int> repository,
            ILogger logger
        ) : base(repository, logger)
        {

            UserRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        }

        public async Task AddUser(int groupId, int userId, CancellationToken cancellationToken = default)
        {
            Group group = await GetByIdAsync(groupId, false, null, cancellationToken);

            User user = (await UserRepo.FetchAsync(new UserQuery
            {
                Ids = new[] { userId },
                IsDeleted = false,
                Includes = new[] { nameof(User.Groups) },
                Take = 1
            }, true, cancellationToken)).First();

            if (user.Groups.Any(e => e.GroupId == groupId))
                return;

            user.Groups.Add(new GroupUser { GroupId = groupId, UserId = userId });
            await UserRepo.SaveAsync(user, cancellationToken);
        }

        public async Task RemoveUser(int groupId, int userId, CancellationToken cancellationToken = default)
        {
            Group group = await GetByIdAsync(groupId, false, null, cancellationToken);

            User user = (await UserRepo.FetchAsync(new UserQuery
            {
                Ids = new[] { userId },
                Includes = new[] { nameof(User.Groups) },
                Take = 1
            }, false, cancellationToken)).FirstOrDefault();

            GroupUser groupUser = user?.Groups.FirstOrDefault(g => g.GroupId == groupId);
            if (groupUser == null)
                return;

            user.Groups.Remove(groupUser);
            await UserRepo.SaveAsync(user, cancellationToken);
        }
    }
}
