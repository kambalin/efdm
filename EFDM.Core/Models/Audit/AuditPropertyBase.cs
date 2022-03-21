using EFDM.Abstractions.Audit;
using EFDM.Core.Models.Domain;
using System;

namespace EFDM.Core.Models.Audit {

    public abstract class AuditPropertyBase<TKey, TEventKey> : IdKeyEntityBase<TKey>, IAuditPropertyBase<TKey, TEventKey>
       where TKey : IComparable, IEquatable<TKey>
       where TEventKey : IComparable, IEquatable<TKey> {

        public TEventKey AuditId { get; set; }
        public string Name { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
