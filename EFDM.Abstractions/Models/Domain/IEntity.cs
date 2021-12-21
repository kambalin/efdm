using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Abstractions.Models.Domain {

    public interface IEntity {
        object Id { get; set; }
        bool IsNew { get; }
    }
}
