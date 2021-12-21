using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EFDM.Abstractions.Models.Domain {

    public interface IUser : IIdKeyEntity<int>, ITitleEntity {        
        public string Login { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
    }
}
