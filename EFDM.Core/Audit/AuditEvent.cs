using EFDM.Abstractions.Audit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EFDM.Core.Audit {

    public class AuditEvent : IAuditEvent {
        public string ContextId { get; set; }        
        public List<IEventEntry> Entries { get; set; }
        public int Result { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
