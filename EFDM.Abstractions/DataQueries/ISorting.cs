using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Abstractions.DataQueries {

    public interface ISorting {
        bool Desc { get; set; }
        string Field { get; set; }
    }
}
