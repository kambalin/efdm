using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Abstractions.Models.Domain {

    public interface IEntityBase<TKey> : IAuditableEntity, IIdKeyEntity<TKey>
        where TKey : IComparable, IEquatable<TKey> {
    }
}
