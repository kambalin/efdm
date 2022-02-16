using EFDM.Abstractions.DAL.Repositories;
using EFDM.Core.Services.Domain;
using EFDM.Test.Core.DataQueries.Models;
using EFDM.Test.Core.Models.Domain;
using EFDM.Test.Core.Services.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace EFDM.Test.Core.Services.Domain {

    public class GroupService : DomainServiceBase<Group, GroupQuery, int, IRepository<Group, int>>, IGroupService {
        
        readonly IRepository<User, int> UserRepo;

        public GroupService(            
            IRepository<User, int> userRepo,
            IRepository<Group, int> repository,
            ILogger logger
        ) : base(repository, logger) {

            UserRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        }

        public void AddUser(int groupId, int userId) {
            Group group = GetById(groupId);

            User user = UserRepo.Fetch(new UserQuery {
                Ids = new[] { userId },
                IsDeleted = false,
                Includes = new[] { nameof(User.Groups) },
                Take = 1
            }, true).First();

            if (user.Groups.Any(e => e.GroupId == groupId))
                return;

            user.Groups.Add(new GroupUser { GroupId = groupId, UserId = userId });
            UserRepo.Save(user);
        }

        public void RemoveUser(int groupId, int userId) {
            Group group = GetById(groupId);

            User user = UserRepo.Fetch(new UserQuery {
                Ids = new[] { userId },
                Includes = new[] { nameof(User.Groups) },
                Take = 1
            }).FirstOrDefault();

            GroupUser groupUser = user?.Groups.FirstOrDefault(g => g.GroupId == groupId);
            if (groupUser == null)
                return;

            user.Groups.Remove(groupUser);
            UserRepo.Save(user);
        }
    }
}
