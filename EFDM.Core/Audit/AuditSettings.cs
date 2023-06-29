using EFDM.Abstractions.Audit;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace EFDM.Core.Audit
{
    public class AuditSettings : IAuditSettings
    {
        public bool Enabled { get; set; }
        public ConcurrentDictionary<string, byte> GlobalIgnoredProperties { get; set; } = new ConcurrentDictionary<string, byte>();
        public ConcurrentDictionary<Type, byte> IncludedTypes { get; set; } = new ConcurrentDictionary<Type, byte>();
        public ConcurrentDictionary<Type, List<int>> ExcludedTypeStateActions { get; set; } = new ConcurrentDictionary<Type, List<int>>();
        public ConcurrentDictionary<Type, HashSet<string>> IgnoredTypeProperties { get; set; } = new ConcurrentDictionary<Type, HashSet<string>>();
        public ConcurrentDictionary<Type, HashSet<string>> OnlyIncludedTypeProperties { get; set; } = new ConcurrentDictionary<Type, HashSet<string>>();
    }
}
