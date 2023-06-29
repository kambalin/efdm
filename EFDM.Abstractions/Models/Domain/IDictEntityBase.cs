using System;

namespace EFDM.Abstractions.Models.Domain
{
    public interface IDictEntityBase<TKey> : ITitleEntity, IEntityBase<TKey>
        where TKey : IComparable, IEquatable<TKey>
    {
    }
}
