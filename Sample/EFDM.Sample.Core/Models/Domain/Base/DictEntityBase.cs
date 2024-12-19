using EFDM.Abstractions.Models.Domain;
using System;

namespace EFDM.Sample.Core.Models.Domain.Base
{
    public abstract class DictEntityBase<TKey> : DomainEntityBase<TKey>, IDictEntityBase<TKey>
        where TKey : IComparable, IEquatable<TKey>
    {
        public virtual string Title { get; set; }
    }
}
