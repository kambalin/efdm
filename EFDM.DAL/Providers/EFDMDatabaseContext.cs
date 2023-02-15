using EFCore.BulkExtensions;
using EFDM.Abstractions.Audit;
using EFDM.Abstractions.DAL.Providers;
using EFDM.Abstractions.Models.Domain;
using EFDM.Core.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EFDM.Core.DAL.Providers {

    public abstract class EFDMDatabaseContext : DbContext, IAuditableDBContext {

        #region fields & properties

        public abstract int ExecutorId { get; protected set; }
        public DateTime? CommitTime { get; set; }
        public IDBContextAuditor Auditor {
            get { return _auditor; }
        }
        public DbContext DbContext { get { return this; } }

        protected readonly string ConnectionString;
        private readonly ILoggerFactory _loggerFactory;
        private IDBContextAuditor _auditor;

        #endregion fields & properties        

        #region constructors

        public EFDMDatabaseContext(DbContextOptions options, ILoggerFactory loggerFactory = null,
            IAuditSettings auditSettings = null) : base(options) {

            _loggerFactory = loggerFactory;
            InitAuditor(auditSettings);
        }

        public EFDMDatabaseContext(string connectionString, IAuditSettings auditSettings = null) {
            ConnectionString = connectionString;
            InitAuditor(auditSettings);
        }

        #endregion constructors

        #region audit config

        public abstract void InitAuditMapping();

        protected virtual void PreSaveActions() {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>()) {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified) {
                    PreSaveDateAuditValues(entry.Entity);
                    PreSavePrincipalAuditValues(entry.Entity);
                    switch (entry.State) {                        
                        case EntityState.Modified:
                            var auditDateEntity = entry.Entity as IAuditableDateEntity;
                            var auditPrincipalEntity = entry.Entity as IAuditablePrincipalEntity;
                            if (auditDateEntity != null)
                                entry.Property($"{nameof(auditDateEntity.Created)}").IsModified = false;
                            if (auditPrincipalEntity != null)
                                entry.Property($"{nameof(auditPrincipalEntity.CreatedById)}").IsModified = false;
                            break;                        
                    }
                }
            }
        }

        protected virtual void PreSavePrincipalAuditValues<TEntity>(TEntity entity) {
            var auditPrincipalEntity = entity as IAuditablePrincipalEntity;
            if (auditPrincipalEntity == null)
                return;
            if (!auditPrincipalEntity.PreserveLastModifiedBy)
                auditPrincipalEntity.ModifiedById = ExecutorId;
            else {
                // if PreserveLastModifiedBy and ModifiedBy not set
                // then "hide" who actually modified to createdby
                if (auditPrincipalEntity.ModifiedById < 1)
                    auditPrincipalEntity.ModifiedById = auditPrincipalEntity.CreatedById;
            }
            // if the creator was not forcibly set (for example, when it has to be system user)
            if (auditPrincipalEntity.CreatedById < 1) 
                auditPrincipalEntity.CreatedById = ExecutorId;
        }

        protected virtual void PreSaveDateAuditValues<TEntity>(TEntity entity) {
            var auditDateEntity = entity as IAuditableDateEntity;
            if (auditDateEntity == null)
                return;
            var modified = DateTime.Now;
            if (CommitTime.HasValue)
                modified = CommitTime.Value;
            if (auditDateEntity.Created == DateTimeOffset.MinValue)
                auditDateEntity.Created = modified;
            if (!auditDateEntity.PreserveLastModified || auditDateEntity.Modified == DateTimeOffset.MinValue)
                auditDateEntity.Modified = modified;
        }

        protected virtual void InitAuditor(IAuditSettings auditSettings = null) {
            _auditor = new DBContextAuditor(this, auditSettings);
            SetDefaultGlobalIgnoredProperties();
            InitAuditMapping();
        }

        protected virtual void SetDefaultGlobalIgnoredProperties() {
            Auditor.ExcludeProperty<IAuditableDateEntity>(x => x.Created);
            Auditor.ExcludeProperty<IAuditablePrincipalEntity>(x => x.CreatedBy);
            Auditor.ExcludeProperty<IAuditablePrincipalEntity>(x => x.CreatedById);
            Auditor.ExcludeProperty<IAuditableDateEntity>(x => x.Modified);
            Auditor.ExcludeProperty<IAuditablePrincipalEntity>(x => x.ModifiedBy);
            Auditor.ExcludeProperty<IAuditablePrincipalEntity>(x => x.ModifiedById);
        }

        #endregion audit config

        #region context config

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (_loggerFactory != null)
                optionsBuilder.UseLoggerFactory(_loggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        #endregion context config

        public void SetExecutor(int id) {
            ExecutorId = id;
        }

        public override int SaveChanges() {
            PreSaveActions();
            return Auditor.SaveChanges(() => base.SaveChanges());
        }

        protected int BaseSaveChanges() {
            return base.SaveChanges();
        }

        public int SaveChanges<TEntity>(bool keepExcludedOriginals = false) where TEntity : class {
            List<IGrouping<EntityState, Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry>> original = null;

            if (keepExcludedOriginals) {
                original = ChangeTracker.Entries()
                        .Where(x => !typeof(TEntity).IsAssignableFrom(x.Entity.GetType()) && x.State != EntityState.Unchanged)
                        .GroupBy(x => x.State)
                        .ToList();
            }

            foreach (var entry in ChangeTracker.Entries().Where(x => !typeof(TEntity).IsAssignableFrom(x.Entity.GetType()))) {
                entry.State = EntityState.Unchanged;
            }

            var affectedRows = SaveChanges();

            if (keepExcludedOriginals) {
                foreach (var state in original) {
                    foreach (var entry in state) {
                        entry.State = state.Key;
                    }
                }
            }

            return affectedRows;
        }

        public void BulkInsertWithPreSave<TEntity>(IList<TEntity> entities, BulkConfig config) 
            where TEntity : class {

            foreach (TEntity entity in entities) {
                PreSaveDateAuditValues(entity);
                PreSavePrincipalAuditValues(entity);
            }
            this.BulkInsert(entities, config);
        }

        public void ClearChangeTracker() {
            ChangeTracker.Clear();
        }

        #region IDisposable

        public override void Dispose() {
            base.Dispose();
        }

        #endregion IDisposable
    }
}
