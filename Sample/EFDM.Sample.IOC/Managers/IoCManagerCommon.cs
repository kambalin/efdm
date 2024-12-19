using EFDM.Abstractions.DAL.Repositories;
using EFDM.Core.Audit;
using EFDM.Core.Constants;
using EFDM.Core.DAL.Providers;
using EFDM.DAL.Converters;
using EFDM.Sample.Core.Constants.System;
using EFDM.Sample.Core.Models.Domain;
using EFDM.Sample.Core.Services.Domain;
using EFDM.Sample.Core.Services.Domain.Interfaces;
using EFDM.Sample.DAL.Providers;
using EFDM.Sample.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace EFDM.Sample.IOC.Managers
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
            // mssql
            //services.AddScoped(provider => new TestDatabaseContext(
            //    GetMssqlDbOptions(provider, configuration), provider.GetService<ILoggerFactory>(), auditSettings
            //));
            // postgres
            services.AddScoped(provider => new TestDatabaseContext(
                GetPgDbOptions(provider, configuration), provider.GetService<ILoggerFactory>(), auditSettings,
                (ModelConfigurationBuilder configurationBuilder) =>
                {
                    configurationBuilder
                        .Properties<DateTimeOffset>()
                        .HaveConversion<DateTimeOffsetConverterUtc>();
                }
            ));
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

        private static DbContextOptions<TestDatabaseContext> GetPgDbOptions(IServiceProvider provider, IConfiguration configuration)
        {
            return new DbContextOptionsBuilder<TestDatabaseContext>()
                .UseNpgsql(configuration.GetSection(SettingsValuesNames.ConnectionStringPg)?.Get<string>())
                .Options;
        }

        private static DbContextOptions<TestDatabaseContext> GetMssqlDbOptions(IServiceProvider provider, IConfiguration configuration)
        {
            return new DbContextOptionsBuilder<TestDatabaseContext>()
                .UseSqlServer(configuration.GetSection(SettingsValuesNames.ConnectionStringMssql)?.Get<string>())
                //.EnableSensitiveDataLogging()
                //.UseLoggerFactory(provider.GetService<ILoggerFactory>())
                //.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                .Options;
        }

        #endregion utils
    }
}
