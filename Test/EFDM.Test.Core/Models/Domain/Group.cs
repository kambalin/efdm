using EFDM.Test.Core.Models.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Test.Core.Models.Domain {

    public class Group : DictIntDeletableEntity {
        public int TypeId { get; set; }
        public virtual GroupType Type { get; set; }
        public virtual ICollection<GroupUser> Users { get; set; }
    }
}
