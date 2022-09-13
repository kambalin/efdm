using EFDM.Abstractions.Models.Domain;
using System;

namespace EFDM.Test.Core.Models.Domain.Base {

    public abstract class DeletableEntityBase<TKey> : DomainEntityBase<TKey>, IEntityBase<TKey>, IDeletableEntity
        where TKey : IComparable, IEquatable<TKey> {

        public bool IsDeleted { get; set; }
    }
}
