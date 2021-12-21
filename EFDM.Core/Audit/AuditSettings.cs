using EFDM.Abstractions.Audit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EFDM.Core.Audit {

    public class AuditSettings : IAuditSettings {
        public bool AuditDisabled { get; set; }
        public HashSet<string> GlobalIgnoredProperties { get; set; } = new HashSet<string>();
        public HashSet<Type> IncludedTypes { get; set; } = new HashSet<Type>();
        public Dictionary<Type, List<int>> ExcludedTypeStateActions { get; set; } = new Dictionary<Type, List<int>>();
    }
}
