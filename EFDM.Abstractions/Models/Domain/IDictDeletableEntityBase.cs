using System;

namespace EFDM.Abstractions.Models.Domain
{
    public interface IDictDeletableEntityBase<TKey> : IDictEntityBase<TKey>, IDeletableEntity
        where TKey : IComparable, IEquatable<TKey>
    {
    }
}
