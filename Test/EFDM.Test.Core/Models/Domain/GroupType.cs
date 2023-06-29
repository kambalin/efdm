using EFDM.Test.Core.Models.Domain.Base;
using System.Collections.Generic;

namespace EFDM.Test.Core.Models.Domain
{
    public class GroupType : DictIntDeletableEntity
    {
        public virtual ICollection<Group> Groups { get; set; }
    }
}
