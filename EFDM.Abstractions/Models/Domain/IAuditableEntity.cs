namespace EFDM.Abstractions.Models.Domain {

    public interface IAuditableEntity  {
        bool PreserveLastModifiedInfo { set; get; }
    }
}
