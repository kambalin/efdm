using EFDM.Abstractions.Audit;
using EFDM.Core.Constants;
using EFDM.Core.DAL.Providers;
using EFDM.Core.Extensions;
using EFDM.Test.Core.Constants.ModelValues;
using EFDM.Test.Core.Models.Audit;
using EFDM.Test.Core.Models.Domain;
using EFDM.Test.Core.Models.Domain.Interfaces;
using EFDM.Test.DAL.Configs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace EFDM.Test.DAL.Providers {

    public class TestDatabaseContext : EFDMDatabaseContext {

        #region fields & props

        public override int ExecutorId { get; protected set; } = UserVals.System.Id;

        #endregion fields & props

        #region dbsets

        public DbSet<User> Users { get; set; }
        public DbSet<GroupType> GroupTypes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<AuditGroupEvent> AuditGroupEvents { get; set; }
        public DbSet<AuditGroupProperty> AuditGroupProperties { get; set; }

        #endregion dbsets

        #region constructors

        public TestDatabaseContext(DbContextOptions<TestDatabaseContext> options,
            ILoggerFactory factory = null, IAuditSettings auditSettings = null)
            : base(options, factory, auditSettings) {
        }

        public TestDatabaseContext(string connectionString, IAuditSettings auditSettings = null)
            : base(connectionString, auditSettings) {
        }

        #endregion constructors

        #region context config

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            //optionsBuilder.EnableSensitiveDataLogging(true);
            base.OnConfiguring(optionsBuilder);
            
            //if (!string.IsNullOrEmpty(ConnectionString))
            //    optionsBuilder.UseSqlServer(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            foreach (var entityType in builder.Model.GetEntityTypes()) {
                builder.ApplyConfiguration<IAuditableUserEntity>(typeof(AuditableUserEntityConfig<>), entityType.ClrType);
            }
        }

        #endregion context config

        #region audit config

        public override void InitAuditMapping() {
            Auditor.Map<GroupUser, AuditGroupEvent, AuditGroupProperty>(
                (auditEvent, entry, eventEntity) => {
                    eventEntity.ObjectId = entry.GetEntry().Entity.GetPropValue($"{nameof(GroupUser.GroupId)}").ToString();
                }
            );
            Auditor.Map<Group, AuditGroupEvent, AuditGroupProperty>(
                (auditEvent, entry, eventEntity) => {
                    eventEntity.ObjectId = entry.GetEntry().Entity.GetPropValue("Id").ToString();
                }
            );
            Auditor.SetEventCommonAction<IAuditEventBase<long>>((auditEvent, entry, eventEntity) => {
                eventEntity.ActionId = entry.Action;
                eventEntity.CreatedById = ExecutorId;
                eventEntity.ObjectType = entry.EntityType.Name;
                eventEntity.Created = DateTimeOffset.Now;

                Add(eventEntity);
                BaseSaveChanges();

                Func<IAuditPropertyBase<long, long>> createPropertyEntity = () => {
                    var res = (Activator.CreateInstance(Auditor.GetPropertyType(entry.EntityType))) as IAuditPropertyBase<long, long>;
                    res.AuditId = eventEntity.Id;
                    return res;
                };
                Action<IAuditPropertyBase<long, long>> savePropertyEntity = (pe) => {
                    if (string.IsNullOrEmpty(pe.Name))
                        return;
                    Add(pe);
                    BaseSaveChanges();
                };
                switch (entry.Action) {
                    case AuditStateActionVals.Insert:
                        foreach (var columnVal in entry.ColumnValues) {
                            var propertyEntity = createPropertyEntity();
                            propertyEntity.Name = columnVal.Key;
                            propertyEntity.NewValue = Convert.ToString(columnVal.Value);
                            savePropertyEntity(propertyEntity);
                        }
                        break;
                    case AuditStateActionVals.Delete:
                        foreach (var columnVal in entry.ColumnValues) {
                            var propertyEntity = createPropertyEntity();
                            propertyEntity.Name = columnVal.Key;
                            propertyEntity.OldValue = Convert.ToString(columnVal.Value);
                            savePropertyEntity(propertyEntity);
                        }
                        break;
                    case AuditStateActionVals.Update:
                        foreach (var change in entry.Changes) {
                            var propertyEntity = createPropertyEntity();
                            propertyEntity.Name = change.ColumnName;
                            propertyEntity.NewValue = Convert.ToString(change.NewValue);
                            propertyEntity.OldValue = Convert.ToString(change.OriginalValue);
                            savePropertyEntity(propertyEntity);
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
