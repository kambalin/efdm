using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Abstractions.Models.Domain {

    public interface IAuditableEntity {
        DateTimeOffset Created { get; set; }
        DateTimeOffset Modified { get; set; }
        int CreatedById { get; set; }
        int ModifiedById { get; set; }
        IUser CreatedBy { get; set; }
        IUser ModifiedBy { get; set; }
        bool PreserveLastModifiedInfo { set; get; }
    }
}
