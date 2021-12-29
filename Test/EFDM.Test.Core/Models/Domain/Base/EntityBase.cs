using EFDM.Abstractions.Models.Domain;
using EFDM.Test.Core.Models.Domain.Interfaces;
using System;

namespace EFDM.Test.Core.Models.Domain.Base {

    public abstract class EntityBase<TKey> : EFDM.Core.Models.Domain.EntityBase<TKey>,
        IAuditableUserEntity
        where TKey : IComparable, IEquatable<TKey> {

        public new User CreatedBy { get; set; }
        public new User ModifiedBy { get; set; }
    }
}
