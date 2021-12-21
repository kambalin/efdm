using EFDM.Abstractions.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Core.Audit {

    public class EventEntryChange : IEventEntryChange {
        public string ColumnName { get; set; }
        public object OriginalValue { get; set; }
        public object NewValue { get; set; }
    }
}
