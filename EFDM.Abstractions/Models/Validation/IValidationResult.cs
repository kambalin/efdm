using System.Collections.Generic;

namespace EFDM.Abstractions.Models.Validation {

    public interface IValidationResult {
        bool IsValid { get; }
        IEnumerable<IValidationError> Errors { get; }
        void Add(params IValidationError[] errors);
        void Add(string message);
        void Add(string field, string message);
        void Add(string block, int index, string field, string message);
        void Add(IValidationResult validation);
        string ToString();
    }
}
