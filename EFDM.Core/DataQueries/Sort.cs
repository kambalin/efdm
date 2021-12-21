using EFDM.Abstractions.DataQueries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Core.DataQueries {

    public class Sort : ISorting {
        public bool Desc { get; set; }
        public string Field { get; set; }
    }
}
