using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Abstractions.Audit {

    public interface IEventEntryChange {
        string ColumnName { get; set; }
        object OriginalValue { get; set; }
        object NewValue { get; set; }
    }
}
