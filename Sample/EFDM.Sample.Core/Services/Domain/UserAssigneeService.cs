using EFDM.Abstractions.DAL.Repositories;
using EFDM.Core.Services.Domain;
using EFDM.Sample.Core.Constants.ModelValues;
using EFDM.Sample.Core.DataQueries.Models;
using EFDM.Sample.Core.Models.Domain;
using EFDM.Sample.Core.Services.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFDM.Sample.Core.Services.Domain;

public class UserAssigneeService :
        DomainServiceBase<UserAssignee, UserAssigneeQuery, int, IRepository<UserAssignee, int>>, IUserAssigneeService
{
    #region fields & properties

    #endregion fields & properties

    #region constructors

    public UserAssigneeService(
        IRepository<UserAssignee, int> repository,
        ILogger logger
    ) : base(repository, logger)
    {
    }

    #endregion constructors

    public UserAssignee? GetAssignee(int typeId, int objectId)
    {
        return FindAssignee(typeId, objectId) ?? null;
    }

    public List<UserAssignee> GetAssignees(int typeId, int objectId, bool onlyActive = false)
    {
        return onlyActive
            ? FindAssignees(typeId, objectId).Where(x => x.Active).ToList()
            : FindAssignees(typeId, objectId);
    }

    public UserAssignee? CreateAssignee(UserAssignee userAssignee)
    {
        var newAssignee = new UserAssignee
        {
            UserId = userAssignee.UserId,
            TypeId = userAssignee.TypeId,
            ObjectId = userAssignee.ObjectId,
            Active = true,
            Data = userAssignee.Data,
            Created = DateTimeOffset.Now,
            StartDate = userAssignee.StartDate,
            EndDate = userAssignee.EndDate
        };

        Repository.Add(newAssignee);
        Repository.SaveChanges();

        return FindAssignee(userAssignee.TypeId, userAssignee.ObjectId);
    }

    public UserAssignee? UpdateAssignee(UserAssignee userAssignee, bool tracked = false)
    {
        // Поиск существующего назначения
        var assignee = tracked ? userAssignee : FindAssignee(userAssignee.TypeId, userAssignee.ObjectId, true);

        if (assignee == null)
            throw new ArgumentException("Назначение не найдено", "TypeId и ObjectId");

        if (!tracked)
        {
            assignee.TypeId = userAssignee.TypeId;
            assignee.ObjectId = userAssignee.ObjectId;
            assignee.Active = userAssignee.Active;
            assignee.UserId = userAssignee.UserId;
            assignee.Data = userAssignee.Data;
            assignee.StartDate = userAssignee.StartDate;
            assignee.EndDate = userAssignee.EndDate;
        }

        Repository.Update(assignee);
        Repository.SaveChanges();
        return FindAssignee(userAssignee.TypeId, userAssignee.ObjectId);
    }

    public List<UserAssignee> FindByPairs(List<UserAssigneeSearchPair> pairs)
    {
        return Repository.Fetch(new UserAssigneeQuery
        {
            TypeObjectPairs = pairs,
            Includes = new[]
            {
                    $"{nameof(UserAssignee.User)}",
                }
        }).ToList();
    }    

    public List<UserAssignee> GetAssignment1ForObjectAndWorkflow(int objectId, int? assignment1Field1, int? assignment1Field2)
    {
        var assignment = Repository.Fetch(new UserAssigneeQuery
        {
            TypeId = UserAssigneeTypeVals.Assignment1,
            Assignment1Field1 = assignment1Field1,
            Assignment1Field2 = assignment1Field2,
            ObjectId = objectId,
            Includes = new[] { $"{nameof(UserAssignee.User)}" },
        }).ToList();
        return assignment;
    }

    public List<UserAssignee> GetAssignment2ForObjectAndWorkflow(int objectId, int? assignment2Field1)
    {
        var assignment = Repository.Fetch(new UserAssigneeQuery
        {
            TypeId = UserAssigneeTypeVals.Assignment2,            
            Assignment2Field1 = assignment2Field1,
            ObjectId = objectId,
            Includes = new[] { $"{nameof(UserAssignee.User)}" },
        }).ToList();
        return assignment;
    }

    private UserAssignee? FindAssignee(int typeId, long objectId, bool tracking = false)
    {
        return BuildUserAssigneeQuery(typeId, objectId, tracking)
            .Take(1)
            .FirstOrDefault();
    }

    private List<UserAssignee> FindAssignees(int typeId, long objectId, bool tracking = false)
    {
        return BuildUserAssigneeQuery(typeId, objectId, tracking)
            .ToList();
    }

    private IEnumerable<UserAssignee> BuildUserAssigneeQuery(int typeId, long objectId, bool tracking = false)
    {
        return Repository.Fetch(new UserAssigneeQuery
        {
            TypeId = typeId,
            ObjectId = objectId,
            Includes = new[] { $"{nameof(UserAssignee.User)}" }
        }, tracking);
    }
}
