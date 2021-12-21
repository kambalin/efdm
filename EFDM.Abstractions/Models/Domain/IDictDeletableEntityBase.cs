using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Abstractions.Models.Domain {

    public interface IDictDeletableEntityBase<TKey> : IDictEntityBase<TKey>, IDeletableEntity
        where TKey : IComparable, IEquatable<TKey> {
    }
}
