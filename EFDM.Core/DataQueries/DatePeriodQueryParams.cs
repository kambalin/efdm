﻿using System;

namespace EFDM.Core.DataQueries
{
    public class DatePeriodQueryParams
    {
        #region fields & properties

        public DateTime? LessOrEquals { get; set; }
        public DateTime? MoreOrEquals { get; set; }
        public bool? OrIsNull { get; set; }

        #endregion fields & properties
    }
}
