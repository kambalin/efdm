# EFDM
Entity framework data manager

## [NuGet](https://www.nuget.org/packages/EFDM.Core/)

[![NuGet Status](https://img.shields.io/nuget/v/EFDM.Core.svg?style=flat)](https://www.nuget.org/packages/EFDM.Core/)
[![NuGet Count](https://img.shields.io/nuget/dt/EFDM.Core.svg)](https://www.nuget.org/packages/EFDM.Core/)

To install the package run the following command on the Package Manager Console:

```
PM> Install-Package EFDM.Core
```

## Usage
You can find working example in `EFDM.Test.*` projects in `Test` folder

### Base entities
Inherit own entities from EFDM.Core.Models.Domain

```csharp
public class Group : DictIntDeletableEntity
{
	public int TypeId { get; set; }
	public virtual GroupType Type { get; set; }
	public virtual ICollection<GroupUser> Users { get; set; }
}
```

### Query entities
```csharp
public class GroupQuery : DictIntDeletableDataQuery<Group>
{
	public int[] UserIds { get; set; }
	public int[] TypeIds { get; set; }

	public override IQueryFilter<Group> ToFilter() {
		var and = new QueryFilter<Group>();

		if (UserIds?.Any() == true)
			and.Add(x => x.Users.Any(xx => UserIds.Contains(xx.UserId)));

		if (TypeIds?.Any() == true)
			and.Add(x => TypeIds.Contains(x.TypeId));

		return base.ToFilter().Add(and);
	}
}
```

### Domain entity service
Create domain service class for entity

```csharp
public class GroupService : DomainServiceBase<Group, GroupQuery, int, IRepository<Group, int>>, IGroupService
{
    readonly IRepository<User, int> UserRepo;

    public GroupService(
        IRepository<User, int> userRepo,
        IRepository<Group, int> repository,
        ILogger logger
    ) : base(repository, logger)
    {

        UserRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
    }

    public async Task AddUser(int groupId, int userId, CancellationToken cancellationToken = default)
    {
        Group group = await GetByIdAsync(groupId, false, null, cancellationToken);

        User user = (await UserRepo.FetchAsync(new UserQuery
        {
            Ids = new[] { userId },
            IsDeleted = false,
            Includes = new[] { nameof(User.Groups) },
            Take = 1
        }, true, cancellationToken)).First();

        if (user.Groups.Any(e => e.GroupId == groupId))
            return;

        user.Groups.Add(new GroupUser { GroupId = groupId, UserId = userId });
        await UserRepo.SaveAsync(user, cancellationToken);
    }

    public async Task RemoveUser(int groupId, int userId, CancellationToken cancellationToken = default)
    {
        Group group = await GetByIdAsync(groupId, false, null, cancellationToken);

        User user = (await UserRepo.FetchAsync(new UserQuery
        {
            Ids = new[] { userId },
            Includes = new[] { nameof(User.Groups) },
            Take = 1
        }, false, cancellationToken)).FirstOrDefault();

        GroupUser groupUser = user?.Groups.FirstOrDefault(g => g.GroupId == groupId);
        if (groupUser == null)
            return;

        user.Groups.Remove(groupUser);
        await UserRepo.SaveAsync(user, cancellationToken);
    }
}
```

### Database context
Create own database context class inherited from `EFDM.Core.DAL.Providers.EFDMDatabaseContext`

```csharp
public class TestDatabaseContext : EFDMDatabaseContext
{
    #region fields & properties

    public override int ExecutorId { get; protected set; } = UserValues.SystemId;

    #region dbsets

    public DbSet<User> Users { get; set; }    
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupUser> GroupUsers { get; set; }

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
        // see below
    }

    #endregion audit config
}
```

### Audit entities creation in database context
Configure audit entities creation in own database context by overriding InitAuditMapping method

```csharp
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
```
### Configure database audit for entities & properties
On database context creation configure audit for entities & properties

```csharp
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
auditSettings.OnlyIncludedTypeProperties.TryAdd(typeof(TaskAnswer), new HashSet<string>() {
	$"{nameof(TaskAnswer.TextField1)}"
});

services.AddScoped(provider => new TestDatabaseContext(
	GetDbOptions(provider, configuration), provider.GetService<ILoggerFactory>(), auditSettings)
);
services.AddScoped<EFDMDatabaseContext>(sp => sp.GetRequiredService<TestDatabaseContext>());

```
## Examples
You can find examples in `Sample` folder `EFDM.Sample.TestConsole` project
