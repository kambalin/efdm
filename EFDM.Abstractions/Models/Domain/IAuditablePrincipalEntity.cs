namespace EFDM.Abstractions.Models.Domain
{
    public interface IAuditablePrincipalEntity : IAuditableEntity
    {
        int CreatedById { get; set; }
        int ModifiedById { get; set; }
        IUser CreatedBy { get; set; }
        IUser ModifiedBy { get; set; }
    }
}
