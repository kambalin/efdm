using EFDM.Abstractions.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Test.Core.Models.Domain.Interfaces {

    public interface IAuditableUserEntity : IAuditableEntity {
        public new User CreatedBy { get; set; }
        public new User ModifiedBy { get; set; }
    }
}
