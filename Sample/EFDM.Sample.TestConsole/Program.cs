using EFDM.Core.DataQueries;
using EFDM.Core.Models.Domain;
using EFDM.Sample.Core.Constants.ModelValues;
using EFDM.Sample.Core.DataQueries.Models;
using EFDM.Sample.Core.Models.Domain;
using EFDM.Sample.Core.Services.Domain.Interfaces;
using EFDM.Sample.DAL.Providers;
using EFDM.Sample.IOC.Managers;
using EFDM.Sample.TestConsole.Utilities;
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
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EFDM.Sample.TestConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", true, true)
                .Build();
            Console.OutputEncoding = Encoding.UTF8;

            using (var serviceProvider = RegisterServices(config))
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    await AddGroupWithSvc(scope);
                    await AddUserWithSvc(scope);
                    //GetGroupsWithSvc(scope);
                    //ChangeGroupsWithSvc(scope);
                    await ChangeGroupTypeWithSvc(scope);
                    //ChangeGroupUsersWithSvc(scope);
                    await AddNTimesGroupsWithSvc(scope);
                    //ChangeEnabledDBContextAuditor(scope, false);
                    await ChangeNGroupsWithSvc(scope);
                    //ChangeEnabledDBContextAuditor(scope, true);
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
                    //GetUserIds(scope);
                    //TestTaskAnswersValidFromQuery(scope);
                    //TestPrincipalSorts(scope);
                    //TestTaskAnswersValidFromTillOrderQuery(scope);
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

        static async Task TestTaskAnswersValidFromTillOrderQuery(IServiceScope scope)
        {
            var take = 100;
            var taQuery = new TaskAnswerQuery
            {
                Take = take,
                Sorts = new[] {
                    new Sort { Field = nameof(TaskAnswer.ValidTill), Desc = false },
                    new Sort { Field = nameof(TaskAnswer.ValidFrom), Desc = false },
                }
            };
            var taSvc = scope.ServiceProvider.GetRequiredService<ITaskAnswerService>();
            var taskAnswers = await taSvc.FetchAsync(taQuery, false);
            foreach (var ta in taskAnswers)
            {
                Console.WriteLine($"From {ta.ValidFrom}, Till '{ta.ValidTill}'");
            }
        }

        static async Task TestPrincipalSorts(IServiceScope scope)
        {
            var take = 10;
            var gQuery = new GroupQuery
            {
                Includes = new[]
                {
                    nameof(Group.CreatedBy),
                    nameof(Group.ModifiedBy)
                },
                Take = take,
                Sorts = new[] { new Sort { Field = "CreatedBy.Title", Desc = true } },
                //Sorts = new[] { new Sort { Field = "Id", Desc = true } },
                //Sorts = new[] { new Sort { Field = $"{nameof(Group.CreatedBy)}.{nameof(Group.CreatedBy.Title)}", Desc = false } },
                //Sorts = new[] { new Sort { Field = $"{nameof(Group.Title)}", Desc = false } },
            };
            var taSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();
            var groups = await taSvc.FetchAsync(gQuery);
            foreach (var group in groups)
            {
                Console.WriteLine($"Group id {group.Id}, createdby '{group.CreatedBy?.Title}'");
            }
        }

        static async Task TestTaskAnswersValidFromQuery(IServiceScope scope)
        {
            var take = 10;
            var taQuery = new TaskAnswerQuery
            {
                //ValidFromQueryParams = new DatePeriodQueryParams
                //{
                //    MoreOrEquals = new DateTime(2023, 09, 10, 20, 0, 0),
                //    LessOrEquals = new DateTime(2023, 09, 11, 4, 0, 0),
                //    OrIsNull = false
                //},
                ValidFromOffsetQueryParams = new DateOffsetPeriodQueryParams
                {
                    MoreOrEquals = new DateTimeOffset(2023, 09, 10, 22, 0, 0, new TimeSpan(3, 0, 0)),
                    LessOrEquals = new DateTimeOffset(2023, 09, 11, 1, 0, 0, new TimeSpan(3, 0, 0)),
                    OrIsNull = false
                },
                Take = take,
                Sorts = new[] { new Sort { Field = nameof(TaskAnswer.Id), Desc = true } },
            };
            var taSvc = scope.ServiceProvider.GetRequiredService<ITaskAnswerService>();
            var taskAnswers = await taSvc.FetchAsync(taQuery, true);
        }

        static async Task GetUserIds(IServiceScope scope)
        {
            var userSvc = scope.ServiceProvider.GetRequiredService<IUserService>();
            var userQuery = new UserQuery();
            var userIds = (await userSvc.FetchIdsAsync(userQuery)).ToList();
            Console.WriteLine($"Users count '{userIds?.Count}'");
        }

        static async Task TestAuditTaskAnswers(IServiceScope scope)
        {
            var take = 1;
            var taQuery = new TaskAnswerQuery
            {
                Take = take,
                Sorts = new[] { new Sort { Field = nameof(TaskAnswer.Id), Desc = true } },
            };
            var taSvc = scope.ServiceProvider.GetRequiredService<ITaskAnswerService>();
            var taskAnswers = await taSvc.FetchAsync(taQuery, true);

            Random rnd = new Random();
            foreach (var ta in taskAnswers)
            {
                ta.AnswerValue = rnd.Next(0, 100);
                ta.TextField1 = $"textfield1 {DateTime.Now}";
                ta.TextField2 = $"textfield2 {DateTime.Now}";
                //groupSvc.Save(group);
            }
            await taSvc.SaveChangesAsync();
        }

        static async Task AddTaskAnswers(IServiceScope scope)
        {
            var times = 2;
            var sw = new Stopwatch();
            sw.Start();
            var taSvc = scope.ServiceProvider.GetRequiredService<ITaskAnswerService>();
            for (var i = 1; i <= times; i++)
            {
                var ta = new TaskAnswer
                {
                    AnswerValue = i,
                    TextField1 = $"{i}TeField1",
                    TextField2 = $"{i}TeField2",
                };
                await taSvc.SaveAsync(ta);
            }
            sw.Stop();
            Console.WriteLine("Elapsed={0}", sw.Elapsed);
        }

        static async Task InsertUsers(IServiceScope scope)
        {
            var userSvc = scope.ServiceProvider.GetRequiredService<IUserService>();
            var users = new List<User>();
            for (var i = 0; i < 10; i++)
            {
                var user = new User
                {
                    Login = "VM22HV\\testuser" + Guid.NewGuid(),
                    Title = "Test User " + Guid.NewGuid(),
                    Groups = new List<GroupUser>()
                };
                user.Groups.Add(new GroupUser { User = user, GroupId = GroupValues.Users });
                await userSvc.AddAsync(user);
                users.Add(user);
            }
            await userSvc.SaveChangesAsync();
            foreach (var user in users)
            {
                Console.WriteLine($"User {user.Login} id is '{user.Id}'");
            }
        }

        //static void BulkInsertUsers(IServiceScope scope)
        //{
        //    var userSvc = scope.ServiceProvider.GetRequiredService<IUserService>();
        //    var users = new List<User>();
        //    for (var i = 0; i < 10; i++)
        //    {
        //        var user = new User
        //        {
        //            Login = "VM22HV\\testuser" + Guid.NewGuid(),
        //            Title = "Test User " + Guid.NewGuid(),
        //            Groups = new List<GroupUser>()
        //        };
        //        user.Groups.Add(new GroupUser { User = user, GroupId = GroupValues.Users });
        //        users.Add(user);
        //    }
        //    var bulkConfig = new BulkConfig { SetOutputIdentity = true, IncludeGraph = true };
        //    userSvc.BulkInsert(users, bulkConfig);
        //    foreach (var user in users)
        //    {
        //        Console.WriteLine($"User {user.Login} id is '{user.Id}'");
        //    }
        //}

        static async Task UpdateExecuteUsers(IServiceScope scope)
        {
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
            var userQuery = new UserQuery
            {
                GroupId = GroupTypeValues.Users,
            };
            var updatedCount = await userSvc.ExecuteUpdateAsync(userQuery,
                s => s.SetProperty(b => b.Email, b => "Execute@update.net"));
            Console.WriteLine($"Updated count '{updatedCount}'");
        }

        static async Task DeleteExecuteUsers(IServiceScope scope)
        {
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
            var userQuery = new UserQuery
            {
                GroupId = GroupTypeValues.Users,
            };
            var deletedCount = await userSvc.ExecuteDeleteAsync(userQuery);
            Console.WriteLine($"Deleted count '{deletedCount}'");
        }

        static async Task GetUsersFromGroup(IServiceScope scope)
        {
            var userSvc = scope.ServiceProvider.GetRequiredService<IUserService>();
            var userQuery = new UserQuery
            {
                GroupId = GroupValues.Users,
            };
            var users = (await userSvc.FetchAsync(userQuery)).ToList();
            Console.WriteLine($"Users count '{users?.Count}'");
        }

        static async Task TestSplitQueryGroupSvc(IServiceScope scope)
        {
            var groupQuery = new GroupQuery
            {
                Sorts = new[] { new Sort { Field = nameof(Group.Id), Desc = true } },
                Includes = new[] {
                    $"{nameof(Group.CreatedBy)}",
                    $"{nameof(Group.Users)}.{nameof(GroupUser.User)}"
                },
                IsDeleted = false,
                SplitQuery = true
            };
            var groupSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();
            var groups = await groupSvc.FetchAsync(groupQuery);

            foreach (var group in groups)
            {
                Console.WriteLine($"Group '{group.Title}', Users count '{group.Users?.Count}'");
            }

            await groupSvc.SaveChangesAsync();
            Console.WriteLine($"--------------------------");
        }

        static async Task TestTaskAnswerCommentService(IServiceScope scope)
        {
            var entitySvc = scope.ServiceProvider.GetRequiredService<ITaskAnswerCommentService>();
            var taskAnswerSvc = scope.ServiceProvider.GetRequiredService<ITaskAnswerService>();
            // one to one like this
            {
                var newAnswerComment = new TaskAnswerComment();
                newAnswerComment.Id = 1; // existing parent entity with this key value
                newAnswerComment.Comment = "xxx";
                await entitySvc.AddAsync(newAnswerComment); // change to addded state
                await entitySvc.SaveAsync(newAnswerComment); // save to db
            }
            // or like this
            {
                var taskAnswer = await taskAnswerSvc.GetByIdAsync(1, true); // true - attached to context parent entity
                var newAnswerComment = new TaskAnswerComment();
                newAnswerComment.Comment = "xxx";
                newAnswerComment.TaskAnswer = taskAnswer;
                await entitySvc.SaveAsync(newAnswerComment);
            }
        }

        static async Task TestTaskAnswerService(IServiceScope scope)
        {
            var entitySvc = scope.ServiceProvider.GetRequiredService<ITaskAnswerService>();
            var taskAnswer = new TaskAnswer();
            taskAnswer.AnswerValue = 1;
            await entitySvc.SaveAsync(taskAnswer);
        }

        static void TestModelXmlSerialization(IServiceScope scope)
        {
            var xao = new XmlAttributeOverrides();
            xao.Add(typeof(EntityBase<int>), "CreatedBy", new XmlAttributes
            {
                XmlIgnore = true
            });
            xao.Add(typeof(EntityBase<int>), "ModifiedBy", new XmlAttributes
            {
                XmlIgnore = true
            });
            // Use List instead ICollection
            xao.Add(typeof(Group), "Users", new XmlAttributes
            {
                XmlIgnore = true
            });
            // Use List instead ICollection
            xao.Add(typeof(User), "Groups", new XmlAttributes
            {
                XmlIgnore = true
            });
            // Use List instead ICollection
            xao.Add(typeof(GroupType), "Groups", new XmlAttributes
            {
                XmlIgnore = true
            });

            var xmlserializer = new XmlSerializer(typeof(List<Group>), xao);
            Console.WriteLine($"It's ok");
        }

        static void TestDeserialization(IServiceScope scope)
        {
            var queryStr = "{\"Take\":20,\"Skip\":0,\"Sorts\":[{\"Field\":\"Id\",\"Desc\":true}]}";
            var typedQuery = JsonConvert.DeserializeObject<UserQuery>(queryStr ?? string.Empty);
            Console.WriteLine($"typedQuery: {typedQuery.Skip}");
        }

        static void ChangeEnabledDBContextAuditor(IServiceScope scope, bool enabled)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDatabaseContext>();
            dbContext.Auditor.Enabled = enabled;
            Console.WriteLine($"DBContextAuditor enabled: {enabled}");
        }

        static async Task ChangeNGroupsWithSvc(IServiceScope scope)
        {
            var take = 3;
            var sw = new Stopwatch();
            sw.Start();

            var groupQuery = new GroupQuery
            {
                Includes = new[] {
                    $"{nameof(Group.Type)}"
                },
                Take = take,
                Sorts = new[] { new Sort { Field = nameof(Group.Id), Desc = true } },
            };
            var groupSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();
            var groups = await groupSvc.FetchAsync(groupQuery, true);

            foreach (var group in groups)
            {
                group.Title = $"Group_{Guid.NewGuid()}";
                group.TypeId = group.TypeId == GroupTypeValues.Administrators ? GroupTypeValues.Users : GroupTypeValues.Administrators;
                group.TextField1 = $"textfield1 {DateTime.Now}";
                group.TextField2 = $"textfield2 {DateTime.Now}";
                //groupSvc.Save(group);
            }
            await groupSvc.SaveChangesAsync();

            sw.Stop();
            Console.WriteLine("Elapsed={0}", sw.Elapsed);
        }

        static async Task ChangeGroupUsersWithSvc(IServiceScope scope)
        {
            var groupQuery = new GroupQuery
            {
                Sorts = new[] { new Sort { Field = nameof(Group.Id), Desc = true } },
                Includes = new[] { $"{nameof(Group.CreatedBy)}",
                    $"{nameof(Group.ModifiedBy)}",
                    $"{nameof(Group.Users)}"
                }
            };
            var groupSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();
            var group = await groupSvc.GetAsync(groupQuery, true);
            var userSvc = scope.ServiceProvider.GetRequiredService<IUserService>();
            var user = await userSvc.GetAsync(new UserQuery
            {
                Ids = new[] { UserValues.SystemId }
            });

            if (!group.Users.Any(x => x.UserId == user.Id))
            {
                Console.WriteLine($"Adding user {user.Id} to group {group.Id}");
                group.Users.Add(new GroupUser { GroupId = group.Id, UserId = user.Id });
            }
            else
                Console.WriteLine($"User {user.Id} already in group {group.Id}");
            await groupSvc.SaveChangesAsync();
            Console.WriteLine($"--------------------------");

            if (group.Users.Any(x => x.UserId == user.Id))
            {
                Console.WriteLine($"Removing user {user.Id} from group {group.Id}");
                group.Users = group.Users.Where(x => x.UserId != user.Id).ToList();
            }
            else
                Console.WriteLine($"User {user.Id} not in group {group.Id}");

            await groupSvc.SaveChangesAsync();
            Console.WriteLine($"--------------------------");
        }

        static async Task ChangeGroupTypeWithSvc(IServiceScope scope)
        {
            var groupQuery = new GroupQuery
            {
                Includes = new[] { 
                    //$"{nameof(Group.CreatedBy)}", 
                    //$"{nameof(Group.ModifiedBy)}",
                    $"{nameof(Group.Type)}"
                },
                Sorts = new[] { new Sort { Field = nameof(Group.Id), Desc = true } },
            };
            var groupSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();
            var group = await groupSvc.GetAsync(groupQuery, true);
            Console.WriteLine($"Changing {group.Id}, {group.Title}, Created: '{group.CreatedBy?.Title}'");
            group.TypeId = group.TypeId == GroupTypeValues.Administrators ? GroupTypeValues.Users : GroupTypeValues.Administrators;
            await groupSvc.SaveChangesAsync();
            Console.WriteLine($"--------------------------");
        }

        static async Task ChangeGroupsWithSvc(IServiceScope scope)
        {
            var originalTitle = "Администраторы";
            var groupQuery = new GroupQuery
            {
                Title = originalTitle,
                Includes = new[] { $"{nameof(Group.CreatedBy)}",
                    $"{nameof(Group.ModifiedBy)}",
                    $"{nameof(Group.Users)}"
                }
            };
            var groupSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();
            var groups = await groupSvc.FetchAsync(groupQuery, true);
            foreach (var group in groups)
            {
                Console.WriteLine($"Changing {group.Id}, {group.Title}, Created: '{group.CreatedBy.Title}'");
                group.Title = $"{originalTitle} {Guid.NewGuid()}";
            }
            await groupSvc.SaveChangesAsync();
            Console.WriteLine($"--------------------------");
            foreach (var group in groups)
            {
                Console.WriteLine($"Changing {group.Id}, {group.Title}, Created: '{group.CreatedBy.Title}'");
                group.Title = $"{originalTitle}";
            }
            await groupSvc.SaveChangesAsync();
            Console.WriteLine($"--------------------------");
        }

        static async Task GetGroupsWithSvc(IServiceScope scope)
        {
            var groupQuery = new GroupQuery
            {
                Title = "Администраторы",
                Includes = new[] { $"{nameof(Group.CreatedBy)}", $"{nameof(Group.ModifiedBy)}" }
            };
            var groupSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();
            var groups = await groupSvc.FetchAsync(groupQuery);
            foreach (var group in groups)
                Console.WriteLine($"{group.Id}, {group.Title}, Created: '{group.CreatedBy.Title}'");

            Console.WriteLine($"--------------------------");
            groupQuery = new GroupQuery
            {
                TypeIds = new int[] { GroupTypeValues.Users },
                Includes = new[] { $"{nameof(Group.Users)}.{nameof(GroupUser.User)}" }
            };
            groups = await groupSvc.FetchAsync(groupQuery);
            foreach (var group in groups)
            {
                Console.WriteLine($"{group.Id}, {group.Title}");
                if (group.Users?.Count > 0)
                {
                    foreach (var gu in group.Users)
                    {
                        Console.WriteLine($"\t{gu.UserId}, {gu.User.Title}");
                    }
                }
                Console.WriteLine($"****");
            }
        }

        static async Task AddNTimesGroupsWithSvc(IServiceScope scope)
        {
            var times = 2;
            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i <= times; i++)
            {
                var group = new Group
                {
                    Title = $"Group_{i}_{Guid.NewGuid()}",
                    TypeId = GroupTypeValues.Users
                };
                var groupSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();
                await groupSvc.SaveAsync(group);
            }
            sw.Stop();
            Console.WriteLine("Elapsed={0}", sw.Elapsed);
        }

        static async Task AddGroupWithSvc(IServiceScope scope)
        {
            var group = new Group
            {
                Title = $"Group {Guid.NewGuid()}",
                TypeId = GroupTypeValues.Users,
                SubTypeId = 0,
                TextField1 = "TextField1"
            };
            var groupSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();
            await groupSvc.SaveAsync(group);
            Console.WriteLine(group.Id);
        }

        static async Task AddUserWithSvc(IServiceScope scope)
        {
            var groupSvc = scope.ServiceProvider.GetRequiredService<IGroupService>();
            var group = await groupSvc.GetAsync(new GroupQuery
            {
                TypeIds = new int[] { GroupTypeValues.Users }
            });
            var user = new User
            {
                Login = "efdm\\user" + Guid.NewGuid(),
                Title = "Title " + Guid.NewGuid(),
                Groups = new List<GroupUser>()
            };
            user.Groups.Add(new GroupUser { User = user, GroupId = group.Id });
            var userSvc = scope.ServiceProvider.GetRequiredService<IUserService>();
            user = await userSvc.SaveAsync(user);
            Console.WriteLine(user.Id);
        }

        #region di settings

        private static ServiceProvider RegisterServices(IConfigurationRoot config)
        {
            var services = new ServiceCollection();
            services.AddLogging((ILoggingBuilder builder) =>
            {
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
