using EFDM.Abstractions.Models.Domain;

namespace EFDM.Sample.Core.Models.Domain.Interfaces
{
    public interface IAuditableUserEntity : IAuditablePrincipalEntity
    {
        new User CreatedBy { get; set; }
        new User ModifiedBy { get; set; }
    }
}
