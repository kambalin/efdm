using System;

namespace EFDM.Abstractions.Models.Domain
{
    public interface IIdKeyEntity<TKey> : IEntity where TKey : IComparable, IEquatable<TKey>
    {
        new TKey Id { get; set; }
    }
}
