using EFDM.Abstractions.DataQueries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EFDM.Core.DataQueries {

    public class DatePeriodQueryParams {

        #region fields & properties

        public DateTime? LessOrEquals { get; set; }
        public DateTime? MoreOrEquals { get; set; }

        #endregion fields & properties
    }
}
