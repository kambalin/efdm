namespace EFDM.Abstractions.Models.Domain {

    public interface IDeletableEntity {
        bool IsDeleted { get; set; }
    }
}
