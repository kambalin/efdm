using EFDM.Abstractions.Audit;
using System.Collections.Generic;

namespace EFDM.Core.Audit
{
    public class AuditEvent : IAuditEvent
    {
        public string ContextId { get; set; }
        public List<IEventEntry> Entries { get; set; }
        public int Result { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
