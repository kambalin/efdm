﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace EFDM.Abstractions.Audit {

    public interface IAuditSettings {
        bool Enabled { get; set; }
        HashSet<string> GlobalIgnoredProperties { get; set; }
        HashSet<Type> IncludedTypes { get; set; }
        Dictionary<Type, List<int>> ExcludedTypeStateActions { get; set; }
        ConcurrentDictionary<Type, HashSet<string>> IgnoredTypeProperties { get; set; }
    }
}
