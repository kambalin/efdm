using EFDM.Abstractions.DAL.Repositories;
using EFDM.Core.Audit;
using EFDM.Core.Constants;
using EFDM.Core.DAL.Providers;
using EFDM.Test.Core.Constants.System;
using EFDM.Test.Core.Models.Domain;
using EFDM.Test.Core.Services.Domain;
using EFDM.Test.Core.Services.Domain.Interfaces;
using EFDM.Test.DAL.Providers;
using EFDM.Test.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace EFDM.Test.IOC.Managers
{
    public class IoCManagerCommon
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            #region db & repos

            var auditSettings = new AuditSettings()
            {
                Enabled = true,
                IncludedTypes = new ConcurrentDictionary<Type, byte>()
                {
                    [typeof(Group)] = 1,
                    [typeof(GroupUser)] = 1,
                    [typeof(TaskAnswer)] = 1
                },
                ExcludedTypeStateActions = new ConcurrentDictionary<Type, List<int>>()
                {
                    [typeof(Group)] = new List<int>() { AuditStateActionVals.Insert }
                },
                IgnoredTypeProperties = new ConcurrentDictionary<Type, HashSet<string>>(),
                OnlyIncludedTypeProperties = new ConcurrentDictionary<Type, HashSet<string>>()
            };
            auditSettings.IgnoredTypeProperties.TryAdd(typeof(Group), new HashSet<string>()
            {
                $"{nameof(Group.TextField1)}"
            });
            auditSettings.OnlyIncludedTypeProperties.TryAdd(typeof(TaskAnswer), new HashSet<string>()
            {
                $"{nameof(TaskAnswer.TextField1)}"
            });

            services.AddScoped(provider => new TestDatabaseContext(
                GetDbOptions(provider, configuration), provider.GetService<ILoggerFactory>(), auditSettings)
            );
            services.AddScoped<EFDMDatabaseContext>(sp => sp.GetRequiredService<TestDatabaseContext>());
            services.AddScoped(typeof(IRepository<,>), typeof(TestRepository<,>));

            #endregion db & repos

            #region domain services

            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGroupTypeService, GroupTypeService>();
            services.AddScoped<ITaskAnswerService, TaskAnswerService>();
            services.AddScoped<ITaskAnswerCommentService, TaskAnswerCommentService>();


            #endregion domain services
        }

        #region utils

        private static DbContextOptions<TestDatabaseContext> GetDbOptions(IServiceProvider provider, IConfiguration configuration)
        {
            return new DbContextOptionsBuilder<TestDatabaseContext>()
                .UseSqlServer(configuration.GetSection(SettingsValuesNames.ConnectionString)?.Get<string>())
                //.EnableSensitiveDataLogging()
                //.UseLoggerFactory(provider.GetService<ILoggerFactory>())
                //.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                .Options;
        }

        #endregion utils
    }
}
