namespace EFDM.Abstractions.Models.Domain {

    public interface IAuditableEntity {
        bool PreserveLastModified { set; get; }
        bool PreserveLastModifiedBy { set; get; }
        bool PreserveLastModifiedFields { set; }
    }
}
