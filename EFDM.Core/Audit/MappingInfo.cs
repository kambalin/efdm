using EFDM.Abstractions.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Core.Audit {

    public class MappingInfo : IMappingInfo {
        public Type AuditEventType { set; get; }
        public Type AuditPropertyType { set; get; }
        public Func<IAuditEvent, IEventEntry, object, Task<bool>> EventAction { get; set; }
    }
}
