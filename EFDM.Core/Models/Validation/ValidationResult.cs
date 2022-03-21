using EFDM.Abstractions.Models.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFDM.Core.Models.Validation {

    public class ValidationResult<T> : ValidationResult {
        public T Model { get; set; }
    }

    public class ValidationResult : IValidationResult {

        #region fields & properties

        public bool IsValid => !_errors.Any();
        public IEnumerable<IValidationError> Errors => _errors;
        protected readonly List<IValidationError> _errors = new List<IValidationError>();

        #endregion fields & properties

        public void Add(params IValidationError[] errors) {
            _errors.AddRange(errors);
        }

        public void Add(string message) {
            Add(new ValidationError() {
                Message = message
            });
        }

        public void Add(string field, string message) {
            Add(new ValidationError() {
                Field = field,
                Message = message
            });
        }

        public void Add(string block, int index, string field, string message) {
            Add(new ValidationError() {
                Block = block,
                Index = index,
                Field = field,
                Message = message
            });
        }

        public void Add(IValidationResult validation) {
            _errors.AddRange(validation.Errors);
        }

        public override string ToString() {
            if (IsValid) {
                return "No errors";
            }

            var sb = new StringBuilder();

            sb.AppendLine("Errors:");

            foreach (var error in Errors) {
                sb.Append("- ").AppendLine(error.ToString());
            }

            return sb.ToString();
        }
    }
}
