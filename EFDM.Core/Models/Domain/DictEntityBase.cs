using EFDM.Abstractions.Models.Domain;
using System;

namespace EFDM.Core.Models.Domain
{
    public abstract class DictEntityBase<TKey> : EntityBase<TKey>, IDictEntityBase<TKey>
        where TKey : IComparable, IEquatable<TKey>
    {
        public virtual string Title { get; set; }
    }
}
