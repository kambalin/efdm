using EFDM.Test.Core.Models.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Test.Core.Models.Domain {

    public class GroupType : DictIntDeletableEntity {
        public virtual ICollection<Group> Groups { get; set; }
    }
}
