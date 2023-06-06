using EFCore.BulkExtensions;
using EFDM.Core.DataQueries;
using EFDM.Core.Models.Domain;
using EFDM.Test.Core.Constants.ModelValues;
using EFDM.Test.Core.DataQueries.Models;
using EFDM.Test.Core.Models.Domain;
using EFDM.Test.Core.Services.Domain.Interfaces;
using EFDM.Test.DAL.Providers;
using EFDM.Test.IOC.Managers;
using EFDM.Test.TestConsole.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EFDM.Test.TestConsole {

    class Program {

        static void Main(string[] args) {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", true, true)
                .Build();
            Console.OutputEncoding = Encoding.UTF8;

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

                    //ChangeEnabledDBContextAuditor(scope, false);
                    //ChangeNGroupsWithSvc(scope);
                    //ChangeEnabledDBContextAuditor(scope, true);
                    //ChangeNGroupsWithSvc(scope);
                    //TestDeserialization(scope);
                    //TestTaskAnswerService(scope);
                    //TestTaskAnswerCommentService(scope);
                    //TestModelXmlSerialization(scope);
                    //TestSplitQueryGroupSvc(scope);
                    //DeleteExecuteUsers(scope);
                    //GetUsersFromGroup(scope);
                    //UpdateExecuteUsers(scope);                    
                    //BulkInsertUsers(scope);
                    //InsertUsers(scope);
                    //AddTaskAnswers(scope);
                    //TestAuditTaskAnswers(scope);
                    GetUserIds(scope);
                }
            }

            //var task1 = Task.Run(() => {
            //    using (var serviceProvider = RegisterServices(config)) {
            //        using (var scope = serviceProvider.CreateScope()) {                       
            //            ChangeNGroupsWithSvc(scope);
            //        }
            //    }
            //});
            //var task2 = Task.Run(() => {
            //    using (var serviceProvider = RegisterServices(config)) {
            //        using (var scope = serviceProvider.CreateScope()) {
            //            ChangeNGroupsWithSvc(scope);
            //        }
            //    }
            //});

            Console.WriteLine("press any key...");
            Console.ReadKey();
        }

        static void GetUserIds(IServiceScope scope) {
            var userSvc = scope.ServiceProvider.GetRequiredService<IUserService>();
            var userQuery = new UserQuery();
            var userIds = userSvc.FetchIds(userQuery).ToList();
            Console.WriteLine($"Users count '{userIds?.Count}'");
        }

        static void TestAuditTaskAnswers(IServiceScope scope) {
            var take = 1;
            var taQuery = new TaskAnswerQuery {
                Take = take,
                Sorts = new[] { new Sort { Field = nameof(TaskAnswer.Id), Desc = true } },
            };
            var taSvc = scope.ServiceProvider.GetRequiredService<ITaskAnswerService>();
            var taskAnswers = taSvc.Fetch(taQuery, true);

            Random rnd = new Random();
            foreach (var ta in taskAnswers) {
                ta.AnswerValue = rnd.Next(0, 100);
                ta.TextField1 = $"textfield1 {DateTime.Now}";
                ta.TextField2 = $"textfield2 {DateTime.Now}";
                //groupSvc.Save(group);
            }
            taSvc.SaveChanges();
        }

        static void AddTaskAnswers(IServiceScope scope) {
            var times = 2;
            var sw = new Stopwatch();
            sw.Start();
            var taSvc = scope.ServiceProvider.GetRequiredService<ITaskAnswerService>();
            for (var i = 1; i <= times; i++) {
                var ta = new TaskAnswer {
                    AnswerValue = i,
                    TextField1 = $"{i}TeField1",
                    TextField2 = $"{i}TeField2",
                };
                taSvc.Save(ta);
            }
            sw.Stop();
            Console.WriteLine("Elapsed={0}", sw.Elapsed);
        }

        static void InsertUsers(IServiceScope scope) {
            var userSvc = scope.ServiceProvider.GetRequiredService<IUserService>();
            var users = new List<User>();
            for (var i = 0; i < 10; i++) {
                var user = new User {
                    Login = "VM22HV\\testuser" + Guid.NewGuid(),
                    Title = "Test User " + Guid.NewGuid(),
                    Groups = new List<GroupUser>()
                };
                user.Groups.Add(new GroupUser { User = user, GroupId = GroupVals.UserGroupId });
                userSvc.Add(user);
                users.Add(user);
            }
            userSvc.SaveChanges();
            foreach (var user in users) {
                Console.WriteLine($"User {user.Login} id is '{user.Id}'");
            }
        }

        static void BulkInsertUsers(IServiceScope scope) {
            var userSvc = scope.ServiceProvider.GetRequiredService<IUserService>();
            var users = new List<User>();
            for (var i = 0; i < 10; i++) {
                var user = new User {
                    Login = "VM22HV\\testuser" + Guid.NewGuid(),
                    Title = "Test User " + Guid.NewGuid(),
                    Groups = new List<GroupUser>()
                };
                user.Groups.Add(new GroupUser { User = user, GroupId = GroupVals.UserGroupId });
                users.Add(user);
            }
            var bulkConfig = new BulkConfig { SetOutputIdentity = true, IncludeGraph = true };
            userSvc.BulkInsert(users, bulkConfig);
            foreach (var user in users) {
                Console.WriteLine($"User {user.Login} id is '{user.Id}'");
            }
        }

        static void UpdateExecuteUsers(IServiceScope scope) {
            var userSvc = scope.ServiceProvider.GetRequiredService<IUserService>();
            //var userQuery = new UserQuery {
            //    Emails = new string[] { "testuser108@test.ru" },
            //};
            //var userQuery = new UserQuery {
            //    Ids = new int[] { 108 },
            //};
            //var userQuery = new UserQuery {
            //    Logins = new string[] { "VM22HV\\testuser107" },
            //};
            var userQuery = new UserQuery {
                GroupId = GroupTypeVals.Users,
            };
            var updatedCount = userSvc.ExecuteUpdate(userQuery,
                s => s.SetProperty(b => b.Email, b => "Execute@update.net"));
            Console.WriteLine($"Updated count '{updatedCount}'");
        }

        static void DeleteExecuteUsers(IServiceScope scope) {
            var userSvc = scope.ServiceProvider.GetRequiredService<IUserService>();
            //var userQuery = new UserQuery {
            //    Emails = new string[] { "testuser108@test.ru" },
            //};
            //var userQuery = new UserQuery {
            //    Ids = new int[] { 108 },
            //};
            //var userQuery = new UserQuery {
            //    Logins = new string[] { "VM22HV\\testuser107" },
            //};
            var userQuery = new UserQuery {
                GroupId = GroupTypeVals.Users,
            };
            var deletedCount = userSvc.ExecuteDelete(userQuery);
            Console.WriteLine($"Deleted count '{deletedCount}'");
        }

        static void GetUsersFromGroup(IServiceScope scope) {
            var userSvc = scope.ServiceProvider.GetRequiredService<IUserService>();
            var userQuery = new UserQuery {
                GroupId = GroupVals.UserGroupId,
            };
            var users = userSvc.Fetch(userQuery).ToList();
            Console.WriteLine($"Users count '{users?.Count}'");
        }

        static void TestSplitQueryGroupSvc(IServiceScope scope) {
            var groupQuery = new GroupQuery {
                Sorts = new[] { new Sort { Field = nameof(Group.Id), Desc = true } },
                Includes = new[] {
                    $"{nameof(Group.CreatedBy)}",
                    $"{nameof(Group.Users)}.{nameof(GroupUser.User)}"
                },
                IsDeleted = false,
                SplitQuery = true
            };
            var groupSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();
            var groups = groupSvc.Fetch(groupQuery);

            foreach (var group in groups) {
                Console.WriteLine($"Group '{group.Title}', Users count '{group.Users?.Count}'");
            }

            groupSvc.SaveChanges();
            Console.WriteLine($"--------------------------");
        }

        static void TestTaskAnswerCommentService(IServiceScope scope) {
            var entitySvc = scope.ServiceProvider.GetRequiredService<ITaskAnswerCommentService>();
            var taskAnswerSvc = scope.ServiceProvider.GetRequiredService<ITaskAnswerService>();
            // one to one like this
            {
                var newAnswerComment = new TaskAnswerComment();
                newAnswerComment.Id = 1; // existing parent entity with this key value
                newAnswerComment.Comment = "xxx";
                entitySvc.Add(newAnswerComment); // change to addded state
                entitySvc.Save(newAnswerComment); // save to db
            }
            // or like this
            {
                var taskAnswer = taskAnswerSvc.GetById(1, true); // true - attached to context parent entity
                var newAnswerComment = new TaskAnswerComment();
                newAnswerComment.Comment = "xxx";
                newAnswerComment.TaskAnswer = taskAnswer;
                entitySvc.Save(newAnswerComment);
            }
        }

        static void TestTaskAnswerService(IServiceScope scope) {
            var entitySvc = scope.ServiceProvider.GetRequiredService<ITaskAnswerService>();
            var taskAnswer = new TaskAnswer();
            taskAnswer.AnswerValue = 1;
            entitySvc.Save(taskAnswer);
        }

        static void TestModelXmlSerialization(IServiceScope scope) {
            var xao = new XmlAttributeOverrides();
            xao.Add(typeof(EntityBase<int>), "CreatedBy", new XmlAttributes {
                XmlIgnore = true
            });
            xao.Add(typeof(EntityBase<int>), "ModifiedBy", new XmlAttributes {
                XmlIgnore = true
            });
            // Use List instead ICollection
            xao.Add(typeof(Group), "Users", new XmlAttributes {
                XmlIgnore = true
            });
            // Use List instead ICollection
            xao.Add(typeof(User), "Groups", new XmlAttributes {
                XmlIgnore = true
            });
            // Use List instead ICollection
            xao.Add(typeof(GroupType), "Groups", new XmlAttributes {
                XmlIgnore = true
            });

            var xmlserializer = new XmlSerializer(typeof(List<Group>), xao);
            Console.WriteLine($"It's ok");
        }

        static void TestDeserialization(IServiceScope scope) {
            var queryStr = "{\"Take\":20,\"Skip\":0,\"Sorts\":[{\"Field\":\"Id\",\"Desc\":true}]}";
            var typedQuery = JsonConvert.DeserializeObject<UserQuery>(queryStr ?? string.Empty);
            Console.WriteLine($"typedQuery: {typedQuery.Skip}");
        }

        static void ChangeEnabledDBContextAuditor(IServiceScope scope, bool enabled) {
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDatabaseContext>();
            dbContext.Auditor.Enabled = enabled;
            Console.WriteLine($"DBContextAuditor enabled: {enabled}");
        }

        static void ChangeNGroupsWithSvc(IServiceScope scope) {
            var take = 3;
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
                group.TextField1 = $"textfield1 {DateTime.Now}";
                group.TextField2 = $"textfield2 {DateTime.Now}";
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
            foreach (var group in groups)
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
