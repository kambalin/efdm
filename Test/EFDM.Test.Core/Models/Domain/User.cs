using EFDM.Abstractions.Models.Domain;
using EFDM.Test.Core.Models.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EFDM.Test.Core.Models.Domain {

    public class User : DictIntDeletableEntity, IUser {
        public string Login { get; set; }
        public string Email { get; set; }
        public override string Title { get; set; }
        public string Phone { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }

        public virtual ICollection<GroupUser> Groups { get; set; }
    }
}
