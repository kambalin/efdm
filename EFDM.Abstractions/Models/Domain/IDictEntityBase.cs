using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Abstractions.Models.Domain {

    public interface IDictEntityBase<TKey> : ITitleEntity, IEntityBase<TKey>
        where TKey : IComparable, IEquatable<TKey> {
    }
}
