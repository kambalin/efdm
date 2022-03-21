using EFDM.Abstractions.Models.Domain;
using System;

namespace EFDM.Core.Models.Domain {

    public abstract class DictDeletableEntityBase<TKey> : DeletableEntityBase<TKey>, IDictDeletableEntityBase<TKey>
        where TKey : IComparable, IEquatable<TKey> {

        public virtual string Title { get; set; }
    }
}
