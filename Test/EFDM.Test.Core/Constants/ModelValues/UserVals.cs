using EFDM.Test.Core.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Test.Core.Constants.ModelValues {
    
    public class UserVals {
        public static readonly User System = new User { Id = 1, Login = "efdm\\system", Title = "SYSTEM" };
    }
}
