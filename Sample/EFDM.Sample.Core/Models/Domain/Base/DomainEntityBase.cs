﻿using EFDM.Sample.Core.Models.Domain.Interfaces;
using System;

namespace EFDM.Sample.Core.Models.Domain.Base
{
    public abstract class DomainEntityBase<TKey> : EFDM.Core.Models.Domain.EntityBase<TKey>,
        IAuditableUserEntity
        where TKey : IComparable, IEquatable<TKey>
    {
        public User CreatedBy { get; set; }
        public User ModifiedBy { get; set; }
    }
}
