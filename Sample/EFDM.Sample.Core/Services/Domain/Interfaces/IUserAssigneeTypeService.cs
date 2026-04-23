using EFDM.Abstractions.Services.Domain;
using EFDM.Sample.Core.DataQueries.Models;
using EFDM.Sample.Core.Models.Domain;

namespace EFDM.Sample.Core.Services.Domain.Interfaces;

public interface IUserAssigneeTypeService : IDomainService<UserAssigneeType, UserAssigneeTypeQuery, int>
{
}
