using EFDM.Core.DataQueries;
using EFDM.Test.Core.Constants.ModelValues;
using EFDM.Test.DAL.Providers;
using EFDM.Test.Core.DataQueries.Models;
using EFDM.Test.Core.Models.Domain;
using EFDM.Test.Core.Services.Domain.Interfaces;
using EFDM.Test.IOC.Managers;
using EFDM.Test.TestConsole.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace EFDM.Test.TestConsole {

    class Program {

        static void Main(string[] args) {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", true, true)
                .Build();

            using (var serviceProvider = RegisterServices(config)) {
                using (var scope = serviceProvider.CreateScope()) {
                    //AddGroupWithSvc(scope);
                    //AddUserWithSvc(scope);
                    //GetGroupsWithSvc(scope);
                    //ChangeGroupsWithSvc(scope);
                    //ChangeGroupTypeWithSvc(scope);
                    //ChangeGroupUsersWithSvc(scope);
                    //AddNTimesGroupsWithSvc(scope);
                    //ChangeNGroupsWithSvc(scope);

                    ChangeDisabledDBContextAuditor(scope, true);
                    ChangeNGroupsWithSvc(scope);
                    ChangeDisabledDBContextAuditor(scope, false);
                    ChangeNGroupsWithSvc(scope);
                }
            }
            Console.WriteLine("press any key...");
            Console.ReadKey();
        }

        static void ChangeDisabledDBContextAuditor(IServiceScope scope, bool disabled) {
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDatabaseContext>();
            dbContext.Auditor.Disabled = disabled;
            Console.WriteLine($"DBContextAuditor disabled: {disabled}");
        }

        static void ChangeNGroupsWithSvc(IServiceScope scope) {
            var take = 100;
            var sw = new Stopwatch();
            sw.Start();

            var groupQuery = new GroupQuery {
                Includes = new[] {
                    $"{nameof(Group.Type)}"
                },
                Take = take,
                Sorts = new[] { new Sort { Field = nameof(Group.Id), Desc = true } },
            };
            var groupSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();
            var groups = groupSvc.Fetch(groupQuery, true);

            foreach (var group in groups) {
                group.Title = $"Group_{Guid.NewGuid()}";
                group.TypeId = group.TypeId == GroupTypeVals.Admins ? GroupTypeVals.Users : GroupTypeVals.Admins;
                //groupSvc.Save(group);
            }            
            groupSvc.SaveChanges();

            sw.Stop();
            Console.WriteLine("Elapsed={0}", sw.Elapsed);
        }

        static void ChangeGroupUsersWithSvc(IServiceScope scope) {
            var groupQuery = new GroupQuery {
                Sorts = new[] { new Sort { Field = nameof(Group.Id), Desc = true } },
                Includes = new[] { $"{nameof(Group.CreatedBy)}",
                    $"{nameof(Group.ModifiedBy)}",
                    $"{nameof(Group.Users)}"
                }
            };
            var groupSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();
            var group = groupSvc.Get(groupQuery, true);            
            var userSvc = scope.ServiceProvider.GetRequiredService<IUserService>();
            var user = userSvc.Get(new UserQuery {
                Ids = new[] { UserVals.System.Id }
            });
            
            if (!group.Users.Any(x => x.UserId == user.Id)) {
                Console.WriteLine($"Adding user {user.Id} to group {group.Id}");
                group.Users.Add(new GroupUser { GroupId = group.Id, UserId = user.Id });
            }
            else
                Console.WriteLine($"User {user.Id} already in group {group.Id}");
            groupSvc.SaveChanges();
            Console.WriteLine($"--------------------------");

            if (group.Users.Any(x => x.UserId == user.Id)) {
                Console.WriteLine($"Removing user {user.Id} from group {group.Id}");
                group.Users = group.Users.Where(x => x.UserId != user.Id).ToList();
            }
            else
                Console.WriteLine($"User {user.Id} not in group {group.Id}");

            groupSvc.SaveChanges();
            Console.WriteLine($"--------------------------");
        }

        static void ChangeGroupTypeWithSvc(IServiceScope scope) {            
            var groupQuery = new GroupQuery {                
                Includes = new[] { 
                    //$"{nameof(Group.CreatedBy)}", 
                    //$"{nameof(Group.ModifiedBy)}",
                    $"{nameof(Group.Type)}"
                },
                Sorts = new[] { new Sort { Field = nameof(Group.Id), Desc = true } },                
            };
            var groupSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();
            var group = groupSvc.Get(groupQuery, true);            
            Console.WriteLine($"Changing {group.Id}, {group.Title}, Created: '{group.CreatedBy?.Title}'");
            group.TypeId = group.TypeId == GroupTypeVals.Admins ? GroupTypeVals.Users : GroupTypeVals.Admins;
            groupSvc.SaveChanges();
            Console.WriteLine($"--------------------------");
        }

        static void ChangeGroupsWithSvc(IServiceScope scope) {
            var originalTitle = "Администраторы";
            var groupQuery = new GroupQuery {
                Title = originalTitle,
                Includes = new[] { $"{nameof(Group.CreatedBy)}", 
                    $"{nameof(Group.ModifiedBy)}",
                    $"{nameof(Group.Users)}"
                }
            };
            var groupSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();            
            var groups = groupSvc.Fetch(groupQuery, true);
            foreach (var group in groups) {
                Console.WriteLine($"Changing {group.Id}, {group.Title}, Created: '{group.CreatedBy.Title}'");
                group.Title = $"{originalTitle} {Guid.NewGuid()}";
            }
            groupSvc.SaveChanges();
            Console.WriteLine($"--------------------------");
            foreach (var group in groups) {
                Console.WriteLine($"Changing {group.Id}, {group.Title}, Created: '{group.CreatedBy.Title}'");
                group.Title = $"{originalTitle}";
            }
            groupSvc.SaveChanges();
            Console.WriteLine($"--------------------------");
        }

        static void GetGroupsWithSvc(IServiceScope scope) {
            var groupQuery = new GroupQuery {
                Title = "Администраторы",
                Includes = new[] { $"{nameof(Group.CreatedBy)}", $"{nameof(Group.ModifiedBy)}" }
            };
            var groupSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();
            var groups = groupSvc.Fetch(groupQuery);
            foreach(var group in groups)
                Console.WriteLine($"{group.Id}, {group.Title}, Created: '{group.CreatedBy.Title}'");

            Console.WriteLine($"--------------------------");
            groupQuery = new GroupQuery {
                TypeIds = new int[] { GroupTypeVals.Users },
                Includes = new[] { $"{nameof(Group.Users)}.{nameof(GroupUser.User)}" }
            };            
            groups = groupSvc.Fetch(groupQuery);
            foreach (var group in groups) {
                Console.WriteLine($"{group.Id}, {group.Title}");
                if (group.Users?.Count > 0) {
                    foreach (var gu in group.Users) {
                        Console.WriteLine($"\t{gu.UserId}, {gu.User.Title}");
                    }
                }
                Console.WriteLine($"****");
            }
        }

        static void AddNTimesGroupsWithSvc(IServiceScope scope) {
            var times = 2;
            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i <= times; i++) {
                var group = new Group {
                    Title = $"Group_{i}_{Guid.NewGuid()}",
                    TypeId = GroupTypeVals.Users
                };
                var groupSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();
                groupSvc.Save(group);                
            }
            sw.Stop();
            Console.WriteLine("Elapsed={0}", sw.Elapsed);
        }

        static void AddGroupWithSvc(IServiceScope scope) {
            var group = new Group {
                Title = $"Group {Guid.NewGuid()}",
                TypeId = GroupTypeVals.Users
            };
            var groupSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();
            groupSvc.Save(group);
            Console.WriteLine(group.Id);
        }

        static void AddUserWithSvc(IServiceScope scope) {
            var groupSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();
            var group = groupSvc.Get(new GroupQuery { 
                TypeIds = new int[] { GroupTypeVals.Users }
            });
            var user = new User {
                Login = "efdm\\user" + Guid.NewGuid(),
                Title = "Title " + Guid.NewGuid(),
                Groups = new List<GroupUser>()
            };
            user.Groups.Add(new GroupUser { User = user, GroupId = group.Id });
            var userSvc = scope.ServiceProvider.GetRequiredService<IUserService>();
            user = userSvc.Save(user);
            Console.WriteLine(user.Id);
        }

        #region di settings

        private static ServiceProvider RegisterServices(IConfigurationRoot config) {
            var services = new ServiceCollection();
            services.AddLogging((ILoggingBuilder builder) => {
                builder.AddDebug();
            });
            services.AddSingleton<ILogger, ConsoleLogger>();
            services.AddMemoryCache();

            IoCManagerCommon.RegisterServices(services, config);

            return services.BuildServiceProvider(true);
        }

        #endregion di settings
    }
}
