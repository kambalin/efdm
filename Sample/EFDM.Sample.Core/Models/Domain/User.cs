using EFDM.Abstractions.Models.Domain;
using EFDM.Sample.Core.Models.Domain.Base;
using System.Collections.Generic;

namespace EFDM.Sample.Core.Models.Domain
{
    public class User : DictIntDeletableEntity, IUser
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public override string Title { get; set; }
        public string Phone { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }

        public virtual ICollection<GroupUser> Groups { get; set; }
    }
}
