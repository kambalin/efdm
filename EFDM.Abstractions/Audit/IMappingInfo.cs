using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Abstractions.Audit {
    
    public interface IMappingInfo {
        Type AuditEventType { set; get; }
        Type AuditPropertyType { set; get; }
        Func<IAuditEvent, IEventEntry, object, Task<bool>> EventAction { get; set; }
    }
}
