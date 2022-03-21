using System;

namespace EFDM.Abstractions.Models.Domain {

    public interface IEntityBase<TKey> : IIdKeyEntity<TKey>, IAuditableEntity,
        IAuditablePrincipalEntity, IAuditableDateEntity
        where TKey : IComparable, IEquatable<TKey> {
    }
}
