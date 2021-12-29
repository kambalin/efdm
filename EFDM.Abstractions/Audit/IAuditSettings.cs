using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Abstractions.Audit {

    public interface IAuditSettings {
        bool Enabled { get; set; }
        HashSet<string> GlobalIgnoredProperties { get; set; }
        HashSet<Type> IncludedTypes { get; set; }
        Dictionary<Type, List<int>> ExcludedTypeStateActions { get; set; }
    }
}
