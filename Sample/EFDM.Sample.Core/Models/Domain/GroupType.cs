using EFDM.Sample.Core.Models.Domain.Base;
using System.Collections.Generic;

namespace EFDM.Sample.Core.Models.Domain
{
    public class GroupType : DictIntDeletableEntity
    {
        public virtual ICollection<Group> Groups { get; set; }
    }
}
