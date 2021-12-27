using System;
using System.Collections.Generic;
using EFDM.Abstractions.DAL.Repositories;
using EFDM.Core.Audit;
using EFDM.Core.DAL.Providers;
using EFDM.Core.DAL.Repositories;
using EFDM.Test.Core.Constants.System;
using EFDM.Test.Core.Models.Domain;
using EFDM.Test.Core.Services.Domain;
using EFDM.Test.Core.Services.Domain.Interfaces;
using EFDM.Test.DAL.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EFDM.Test.IOC.Managers {

    public class IoCManagerCommon {

        public static void RegisterServices(IServiceCollection services, IConfiguration configuration) {

            #region db & repos

            var auditSettings = new AuditSettings() {
                IncludedTypes = new HashSet<Type>() { 
                    typeof(Group), typeof(GroupUser) 
                },
                ExcludedTypeStateActions = new Dictionary<Type, List<int>>() {
                    //{  typeof(Group), new List<int>() { AuditStateActionVals.Insert } },
                }
            };

            services.AddScoped(provider => new TestDatabaseContext(
                GetDbOptions(provider, configuration), provider.GetService<ILoggerFactory>(), auditSettings)
            );
            services.AddScoped<EFDMDatabaseContext>(sp => sp.GetRequiredService<TestDatabaseContext>());
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

            #endregion db & repos

            #region domain services

            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGroupTypeService, GroupTypeService>();
            
            #endregion domain services
        }

        #region utils

        private static DbContextOptions<TestDatabaseContext> GetDbOptions(IServiceProvider provider, IConfiguration configuration) {
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
