using EFDM.Abstractions.Models.Domain;
using System;

namespace EFDM.Abstractions.Audit
{
    public interface IAuditPropertyBase<TKey, TEventKey> : IIdKeyEntity<TKey>
        where TKey : IComparable, IEquatable<TKey>
        where TEventKey : IComparable, IEquatable<TKey>
    {
        TEventKey AuditId { get; set; }
        string Name { get; set; }
        string OldValue { get; set; }
        string NewValue { get; set; }
    }
}
