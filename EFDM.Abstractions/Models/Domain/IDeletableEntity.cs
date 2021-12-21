using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Abstractions.Models.Domain {

    public interface IDeletableEntity {
        bool IsDeleted { get; set; }
    }
}
