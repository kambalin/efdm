using EFDM.Abstractions.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Abstractions.Audit {

    public interface IAuditEventBase<TKey> : IIdKeyEntity<TKey>
        where TKey : IComparable, IEquatable<TKey> {

        DateTimeOffset Created { get; set; }
        int CreatedById { get; set; }
        string ObjectType { get; set; }
        string ObjectId { get; set; }
        int ActionId { get; set; }
    }
}
