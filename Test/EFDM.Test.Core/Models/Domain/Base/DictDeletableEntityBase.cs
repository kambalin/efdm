using EFDM.Abstractions.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Test.Core.Models.Domain.Base {

    public abstract class DictDeletableEntityBase<TKey> : DeletableEntityBase<TKey>, IDictDeletableEntityBase<TKey>
        where TKey : IComparable, IEquatable<TKey> {

        public virtual string Title { get; set; }
    }
}
