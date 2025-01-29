using EFDM.Abstractions.Audit;
using EFDM.Core.Constants;
using EFDM.Core.DAL.Providers;
using EFDM.Core.Extensions;
using EFDM.Sample.Core.Constants.ModelValues;
using EFDM.Sample.Core.Models.Audit;
using EFDM.Sample.Core.Models.Domain;
using EFDM.Sample.Core.Models.Domain.Interfaces;
using EFDM.Sample.DAL.Configs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace EFDM.Sample.DAL.Providers
{
    public class TestDatabaseContext : EFDMDatabaseContext
    {
        #region fields & properties

        public override int ExecutorId { get; protected set; } = UserValues.SystemId;

        #region dbsets

        public DbSet<User> Users { get; set; }
        public DbSet<GroupType> GroupTypes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<AuditGroupEvent> AuditGroupEvents { get; set; }
        public DbSet<AuditGroupProperty> AuditGroupProperties { get; set; }
        public DbSet<TaskAnswer> TaskAnswers { get; set; }
        public DbSet<TaskAnswerComment> TaskAnswerComments { get; set; }
        public DbSet<AuditTaskAnswerEvent> AuditTaskAnswerEvents { get; set; }
        public DbSet<AuditTaskAnswerProperty> AuditTaskAnswerProperties { get; set; }

        #endregion dbsets

        #endregion fields & properties

        #region constructors

        public TestDatabaseContext(DbContextOptions<TestDatabaseContext> options,
            ILoggerFactory factory = null, IAuditSettings auditSettings = null,
            Action<ModelConfigurationBuilder> conventionsAction = null)
            : base(options, factory, auditSettings, conventionsAction)
        {
        }

        public TestDatabaseContext(string connectionString, IAuditSettings auditSettings = null,
            Action<ModelConfigurationBuilder> conventionsAction = null)
            : base(connectionString, auditSettings, conventionsAction)
        {
        }

        #endregion constructors

        #region context config

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(GetType()));

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                builder.ApplyConfiguration<IAuditableUserEntity>(typeof(AuditableUserEntityConfig<>), entityType.ClrType);
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            if (ConventionsAction != null)
                ConventionsAction(configurationBuilder);
        }

        #endregion context config

        #region audit config

        public override void InitAuditMapping()
        {
            Auditor.Map<GroupUser, AuditGroupEvent, AuditGroupProperty>(
                (auditEvent, entry, eventEntity) =>
                {
                    eventEntity.ObjectId = entry.GetEntry().Entity.GetPropValue($"{nameof(GroupUser.GroupId)}").ToString();
                }
            );
            Auditor.Map<Group, AuditGroupEvent, AuditGroupProperty>(
                (auditEvent, entry, eventEntity) =>
                {
                    eventEntity.ObjectId = entry.GetEntry().Entity.GetPropValue("Id").ToString();
                }
            );
            Auditor.Map<TaskAnswer, AuditTaskAnswerEvent, AuditTaskAnswerProperty>(
                (auditEvent, entry, eventEntity) =>
                {
                    eventEntity.ObjectId = entry.GetEntry().Entity.GetPropValue("Id").ToString();
                }
            );
            Auditor.SetEventCommonAction<IAuditEventBase<long>>(async (auditEvent, entry, eventEntity) =>
            {
                eventEntity.ActionId = entry.Action;
                eventEntity.CreatedById = ExecutorId;
                eventEntity.ObjectType = entry.EntityType.Name;
                eventEntity.Created = DateTimeOffset.Now;

                await AddAsync(eventEntity);
                await BaseSaveChangesAsync();

                Func<IAuditPropertyBase<long, long>> createPropertyEntity = () =>
                {
                    var res = Activator.CreateInstance(Auditor.GetPropertyType(entry.EntityType)) as IAuditPropertyBase<long, long>;
                    res.AuditId = eventEntity.Id;
                    return res;
                };
                switch (entry.Action)
                {
                    case AuditStateActionVals.Insert:
                        foreach (var columnVal in entry.ColumnValues)
                        {
                            var propertyEntity = createPropertyEntity();
                            propertyEntity.Name = columnVal.Key;
                            propertyEntity.NewValue = Convert.ToString(columnVal.Value);
                            if (!string.IsNullOrEmpty(propertyEntity.Name))
                            {
                                await AddAsync(propertyEntity);
                                await BaseSaveChangesAsync();
                            }
                        }
                        break;
                    case AuditStateActionVals.Delete:
                        foreach (var columnVal in entry.ColumnValues)
                        {
                            var propertyEntity = createPropertyEntity();
                            propertyEntity.Name = columnVal.Key;
                            propertyEntity.OldValue = Convert.ToString(columnVal.Value);
                            if (!string.IsNullOrEmpty(propertyEntity.Name))
                            {
                                await AddAsync(propertyEntity);
                                await BaseSaveChangesAsync();
                            }
                        }
                        break;
                    case AuditStateActionVals.Update:
                        foreach (var change in entry.Changes)
                        {
                            var propertyEntity = createPropertyEntity();
                            propertyEntity.Name = change.ColumnName;
                            propertyEntity.NewValue = Convert.ToString(change.NewValue);
                            propertyEntity.OldValue = Convert.ToString(change.OriginalValue);
                            if (!string.IsNullOrEmpty(propertyEntity.Name))
                            {
                                await AddAsync(propertyEntity);
                                await BaseSaveChangesAsync();
                            }
                        }
                        break;
                    default:
                        break;
                }
            });
        }

        #endregion audit config
    }
}
