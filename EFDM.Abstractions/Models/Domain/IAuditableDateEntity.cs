using System;

namespace EFDM.Abstractions.Models.Domain {

    public interface IAuditableDateEntity : IAuditableEntity {
        DateTimeOffset Created { get; set; }
        DateTimeOffset Modified { get; set; }
    }
}
