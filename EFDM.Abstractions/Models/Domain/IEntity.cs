namespace EFDM.Abstractions.Models.Domain
{
    public interface IEntity
    {
        object Id { get; set; }
        bool IsNew { get; }
    }
}
