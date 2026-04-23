using EFDM.Abstractions.DAL.Repositories;
using EFDM.Abstractions.Services.Domain;
using EFDM.Sample.Core.Constants.ModelValues;
using EFDM.Sample.Core.DataQueries.Models;
using EFDM.Sample.Core.Models.Domain;
using System.Collections.Generic;

namespace EFDM.Sample.Core.Services.Domain.Interfaces;

public interface IUserAssigneeService : IDomainService<UserAssignee, UserAssigneeQuery, int>
{
    UserAssignee? GetAssignee(int typeId, int objectId);
    List<UserAssignee> GetAssignees(int typeId, int objectId, bool onlyActive = false);
    UserAssignee? CreateAssignee(UserAssignee userAssignee);
    UserAssignee? UpdateAssignee(UserAssignee userAssignee, bool tracked = false);
    List<UserAssignee> FindByPairs(List<UserAssigneeSearchPair> pairs);
    List<UserAssignee> GetAssignment1ForObjectAndWorkflow(int objectId, int? assignment1Field1, int? assignment1Field2);
    List<UserAssignee> GetAssignment2ForObjectAndWorkflow(int objectId, int? assignment2Field1);
}
