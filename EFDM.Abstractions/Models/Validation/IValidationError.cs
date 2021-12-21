using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Abstractions.Models.Validation {

    public interface IValidationError {
        string Block { get; set; }
        int? Index { get; set; }
        string Field { get; set; }
        string Message { get; set; }
    }
}
