using EFDM.Abstractions.Audit;
using EFDM.Abstractions.DAL.Providers;
using EFDM.Abstractions.Models.Domain;
using EFDM.Core.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            var modified = DateTime.Now;
            if (this.CommitTime.HasValue)
                modified = this.CommitTime.Value;

            foreach (var auditableEntity in ChangeTracker.Entries<IAuditableEntity>()) {
                if (auditableEntity.State == EntityState.Added || auditableEntity.State == EntityState.Modified) {
                    if (!auditableEntity.Entity.PreserveLastModifiedInfo) {
                        auditableEntity.Entity.Modified = modified;
                        auditableEntity.Entity.ModifiedById = this.ExecutorId;
                    }
                    switch (auditableEntity.State) {
                        case EntityState.Added:
                            auditableEntity.Entity.Created = modified;
                            if (auditableEntity.Entity.CreatedById < 1) { // if the creator was not forcibly set (for example, when it has to be system user)
                                auditableEntity.Entity.CreatedById = this.ExecutorId;
                            }
                            else { // if the creator was forcibly set, then "hide" who modified
                                auditableEntity.Entity.ModifiedById = auditableEntity.Entity.CreatedById;
                            }
                            break;
                        case EntityState.Modified:
                            auditableEntity.Property(x => x.Created).IsModified = false;
                            auditableEntity.Property(x => x.CreatedById).IsModified = false;
                            break;
                        case EntityState.Deleted:
                            break;
                    }
                }
            }
        }        

        protected virtual void InitAuditor(IAuditSettings auditSettings = null) {
            _auditor = new DBContextAuditor(this, auditSettings);
            SetDefaultGlobalIgnoredProperties();
            InitAuditMapping();
        }

        protected virtual void SetDefaultGlobalIgnoredProperties() {
            Auditor.ExcludeProperty<IAuditableEntity>(x => x.Created);
            Auditor.ExcludeProperty<IAuditableEntity>(x => x.CreatedBy);
            Auditor.ExcludeProperty<IAuditableEntity>(x => x.CreatedById);
            Auditor.ExcludeProperty<IAuditableEntity>(x => x.Modified);
            Auditor.ExcludeProperty<IAuditableEntity>(x => x.ModifiedBy);
            Auditor.ExcludeProperty<IAuditableEntity>(x => x.ModifiedById);
        }

        #endregion audit config

        #region context config

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.EnableSensitiveDataLogging(true);
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

        public void ResetContextState() {
            ChangeTracker.Entries()
                .Where(e => e.Entity != null).ToList()
                .ForEach(e => e.State = EntityState.Detached);
        }

        #region IDisposable

        public override void Dispose() {
            base.Dispose();
        }

        #endregion IDisposable
    }
}
