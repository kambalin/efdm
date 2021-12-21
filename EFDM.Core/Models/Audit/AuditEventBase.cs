using EFDM.Abstractions.Audit;
using EFDM.Abstractions.Models.Domain;
using EFDM.Core.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Core.Models.Audit {

    public abstract class AuditEventBase<TKey> : IdKeyEntityBase<TKey>, IAuditEventBase<TKey>
       where TKey : IComparable, IEquatable<TKey> {

        public DateTimeOffset Created { get; set; }
        public int CreatedById { get; set; }
        public string ObjectType { get; set; }
        public string ObjectId { get; set; }
        public int ActionId { get; set; }
    }
}
