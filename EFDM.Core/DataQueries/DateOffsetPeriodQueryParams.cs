using System;

namespace EFDM.Core.DataQueries
{
    public class DateOffsetPeriodQueryParams
    {
        #region fields & properties

        public DateTimeOffset? LessOrEquals { get; set; }
        public DateTimeOffset? MoreOrEquals { get; set; }
        public bool? OrIsNull { get; set; }

        #endregion fields & properties
    }
}
