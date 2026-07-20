using EFDM.Abstractions.Audit;
using EFDM.Abstractions.DAL.Providers;
using EFDM.Abstractions.Models.Domain;
using EFDM.Core.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using SysData = System.Data;

namespace EFDM.DAL.Providers;

public abstract class EFDMDatabaseContext : DbContext, IAuditableDBContext
{
    #region fields & properties

    public abstract int ExecutorId { get; protected set; }
    public Action<ModelConfigurationBuilder> ConventionsAction { get; protected set; }
    public DateTime? CommitTime { get; set; }
    public IDBContextAuditor Auditor
    {
        get
        {
            // lazy init: InitAuditMapping is virtual and must not be called from the base constructor,
            // where the derived context is not initialized yet
            if (_auditor == null)
                InitAuditor(_auditSettings);
            return _auditor;
        }
    }
    public DbContext DbContext { get { return this; } }

    protected readonly string ConnectionString;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IAuditSettings _auditSettings;
    private IDBContextAuditor _auditor;

    #endregion fields & properties        

    #region constructors

    public EFDMDatabaseContext(DbContextOptions options, ILoggerFactory loggerFactory = null,
        IAuditSettings auditSettings = null, Action<ModelConfigurationBuilder> conventionsAction = null) : base(options)
    {
        _loggerFactory = loggerFactory;
        _auditSettings = auditSettings;
        ConventionsAction = conventionsAction;
    }

    public EFDMDatabaseContext(string connectionString, IAuditSettings auditSettings = null,
        Action<ModelConfigurationBuilder> conventionsAction = null)
    {
        ConnectionString = connectionString;
        _auditSettings = auditSettings;
        ConventionsAction = conventionsAction;
    }

    #endregion constructors

    #region audit config

    public abstract void InitAuditMapping();

    protected virtual void PreSaveActions()
    {
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                PreSaveDateAuditValues(entry.Entity);
                PreSavePrincipalAuditValues(entry.Entity);
                switch (entry.State)
                {
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

    protected virtual void PreSavePrincipalAuditValues<TEntity>(TEntity entity)
    {
        var auditPrincipalEntity = entity as IAuditablePrincipalEntity;
        if (auditPrincipalEntity == null)
            return;
        // if the creator was not forcibly set (for example, when it has to be system user)
        if (auditPrincipalEntity.CreatedById < 1)
            auditPrincipalEntity.CreatedById = ExecutorId;
        if (!auditPrincipalEntity.PreserveLastModifiedBy)
            auditPrincipalEntity.ModifiedById = ExecutorId;
        else
        {
            // if PreserveLastModifiedBy and ModifiedBy not set
            // then "hide" who actually modified to createdby
            if (auditPrincipalEntity.ModifiedById < 1)
                auditPrincipalEntity.ModifiedById = auditPrincipalEntity.CreatedById;
        }
    }

    protected virtual void PreSaveDateAuditValues<TEntity>(TEntity entity)
    {
        var auditDateEntity = entity as IAuditableDateEntity;
        if (auditDateEntity == null)
            return;
        var modified = DateTimeOffset.Now;
        if (CommitTime.HasValue)
            modified = CommitTime.Value;
        if (auditDateEntity.Created == DateTimeOffset.MinValue)
            auditDateEntity.Created = modified;
        if (!auditDateEntity.PreserveLastModified || auditDateEntity.Modified == DateTimeOffset.MinValue)
            auditDateEntity.Modified = modified;
    }

    protected virtual void InitAuditor(IAuditSettings auditSettings = null)
    {
        _auditor = new DBContextAuditor(this, auditSettings);
        SetDefaultGlobalIgnoredProperties();
        InitAuditMapping();
    }

    protected virtual void SetDefaultGlobalIgnoredProperties()
    {
        Auditor.ExcludeProperty<IAuditableDateEntity>(x => x.Created);
        Auditor.ExcludeProperty<IAuditablePrincipalEntity>(x => x.CreatedBy);
        Auditor.ExcludeProperty<IAuditablePrincipalEntity>(x => x.CreatedById);
        Auditor.ExcludeProperty<IAuditableDateEntity>(x => x.Modified);
        Auditor.ExcludeProperty<IAuditablePrincipalEntity>(x => x.ModifiedBy);
        Auditor.ExcludeProperty<IAuditablePrincipalEntity>(x => x.ModifiedById);
    }

    #endregion audit config

    #region context config

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_loggerFactory != null)
            optionsBuilder.UseLoggerFactory(_loggerFactory);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    #endregion context config

    public void SetExecutor(int id)
    {
        ExecutorId = id;
    }

    public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        => isolationLevel == IsolationLevel.Unspecified
            ? Database.BeginTransaction()
            : Database.BeginTransaction(ToDataIsolationLevel(isolationLevel));

    public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified, CancellationToken cancellationToken = default)
        => isolationLevel == IsolationLevel.Unspecified
            ? Database.BeginTransactionAsync(cancellationToken)
            : Database.BeginTransactionAsync(ToDataIsolationLevel(isolationLevel), cancellationToken);

    private static SysData.IsolationLevel ToDataIsolationLevel(IsolationLevel level) => level switch
    {
        IsolationLevel.ReadCommitted => SysData.IsolationLevel.ReadCommitted,
        IsolationLevel.ReadUncommitted => SysData.IsolationLevel.ReadUncommitted,
        IsolationLevel.RepeatableRead => SysData.IsolationLevel.RepeatableRead,
        IsolationLevel.Serializable => SysData.IsolationLevel.Serializable,
        IsolationLevel.Snapshot => SysData.IsolationLevel.Snapshot,
        IsolationLevel.Chaos => SysData.IsolationLevel.Chaos,
        _ => SysData.IsolationLevel.Unspecified
    };

    public override int SaveChanges()
    {
        PreSaveActions();
        return Auditor.SaveChanges(() => base.SaveChanges());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        PreSaveActions();
        return await Auditor.SaveChangesAsync(async () => await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);
    }

    protected int BaseSaveChanges() => base.SaveChanges();

    protected async Task<int> BaseSaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual int PersistAuditEntries(IEnumerable<object> entities)
    {
        if (entities == null)
            return 0;
        var any = false;
        foreach (var e in entities)
        {
            if (e == null)
                continue;
            base.Add(e);
            any = true;
        }
        if (!any)
            return 0;
        return BaseSaveChanges();
    }

    public virtual async Task<int> PersistAuditEntriesAsync(IEnumerable<object> entities,
        CancellationToken cancellationToken = default)
    {
        if (entities == null)
            return 0;
        var any = false;
        foreach (var e in entities)
        {
            if (e == null)
                continue;
            base.Add(e);
            any = true;
        }
        if (!any)
            return 0;
        return await BaseSaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Selective (partial) commit: saves only the pending changes of <typeparamref name="TEntity"/> entities
    /// (and its derived types), temporarily "freezing" all other changes accumulated in the context.
    /// All tracked entries of other types are switched to the Unchanged state, so the save (and audit)
    /// covers only <typeparamref name="TEntity"/> changes. Afterwards the frozen entries get their original
    /// states (Added/Modified/Deleted) back — their changes stay pending and can be persisted
    /// by a subsequent SaveChanges call.
    /// </summary>
    public async Task<int> SaveChangesOnlyAsync<TEntity>(CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var original = ChangeTracker.Entries()
                .Where(x => !typeof(TEntity).IsAssignableFrom(x.Entity.GetType()) && x.State != EntityState.Unchanged)
                .GroupBy(x => x.State)
                .ToList();

        foreach (var entry in original.SelectMany(x => x))
        {
            entry.State = EntityState.Unchanged;
        }

        try
        {
            return await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            foreach (var state in original)
            {
                foreach (var entry in state)
                {
                    entry.State = state.Key;
                }
            }
        }
    }

    public void ClearChangeTracker()
    {
        ChangeTracker.Clear();
    }
}
