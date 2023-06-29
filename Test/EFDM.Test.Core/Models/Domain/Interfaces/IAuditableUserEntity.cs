using EFDM.Abstractions.Models.Domain;

namespace EFDM.Test.Core.Models.Domain.Interfaces
{
    public interface IAuditableUserEntity : IAuditablePrincipalEntity
    {
        public new User CreatedBy { get; set; }
        public new User ModifiedBy { get; set; }
    }
}
