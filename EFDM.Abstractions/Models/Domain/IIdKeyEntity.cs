using System;

namespace EFDM.Abstractions.Models.Domain
{
    public interface IIdKeyEntity<TKey> where TKey : IComparable, IEquatable<TKey>
    {
        TKey Id { get; set; }
        bool IsNew { get; }
    }
}
