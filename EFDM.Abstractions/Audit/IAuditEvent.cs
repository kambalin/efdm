using System.Collections.Generic;

namespace EFDM.Abstractions.Audit
{
    public interface IAuditEvent
    {
        string ContextId { get; set; }
        List<IEventEntry> Entries { get; set; }
        int Result { get; set; }
        bool Success { get; set; }
        string ErrorMessage { get; set; }
    }
}
