using EFDM.Test.Core.Models.Domain.Base;
using System.Collections.Generic;

namespace EFDM.Test.Core.Models.Domain {

    public class Group : DictIntDeletableEntity {
        public int TypeId { get; set; }
        public virtual GroupType Type { get; set; }
        public virtual ICollection<GroupUser> Users { get; set; }
    }
}
