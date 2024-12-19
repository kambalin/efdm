using EFDM.Sample.Core.Models.Domain.Base;
using System.Collections.Generic;

namespace EFDM.Sample.Core.Models.Domain
{
    public class Group : DictIntDeletableEntity
    {
        public int TypeId { get; set; }
        public virtual GroupType Type { get; set; }
        public virtual ICollection<GroupUser> Users { get; set; }
        public string TextField1 { get; set; }
        public string TextField2 { get; set; }
        public int SubTypeId { get; set; }
    }
}
