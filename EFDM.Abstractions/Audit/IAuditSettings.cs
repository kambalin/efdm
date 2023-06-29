using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace EFDM.Abstractions.Audit
{
    public interface IAuditSettings
    {
        bool Enabled { get; set; }
        ConcurrentDictionary<string, byte> GlobalIgnoredProperties { get; set; }
        ConcurrentDictionary<Type, byte> IncludedTypes { get; set; }
        ConcurrentDictionary<Type, List<int>> ExcludedTypeStateActions { get; set; }
        ConcurrentDictionary<Type, HashSet<string>> IgnoredTypeProperties { get; set; }
        ConcurrentDictionary<Type, HashSet<string>> OnlyIncludedTypeProperties { get; set; }
    }
}
