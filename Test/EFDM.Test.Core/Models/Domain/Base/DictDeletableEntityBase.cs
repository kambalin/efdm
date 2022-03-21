using EFDM.Abstractions.Models.Domain;
using System;

namespace EFDM.Test.Core.Models.Domain.Base {

    public abstract class DictDeletableEntityBase<TKey> : DeletableEntityBase<TKey>, IDictDeletableEntityBase<TKey>
        where TKey : IComparable, IEquatable<TKey> {

        public virtual string Title { get; set; }
    }
}
