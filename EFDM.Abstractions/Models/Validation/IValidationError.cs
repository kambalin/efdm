namespace EFDM.Abstractions.Models.Validation {

    public interface IValidationError {
        string Block { get; set; }
        int? Index { get; set; }
        string Field { get; set; }
        string Message { get; set; }
    }
}
