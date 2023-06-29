using EFDM.Abstractions.Audit;
using EFDM.Core.Models.Domain;
using System;

namespace EFDM.Core.Models.Audit
{
    public abstract class AuditEventBase<TKey> : IdKeyEntityBase<TKey>, IAuditEventBase<TKey>
       where TKey : IComparable, IEquatable<TKey>
    {
        public DateTimeOffset Created { get; set; }
        public int CreatedById { get; set; }
        public string ObjectType { get; set; }
        public string ObjectId { get; set; }
        public int ActionId { get; set; }
    }
}
