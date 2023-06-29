using System;
using System.Collections.Generic;

namespace EFDM.Abstractions.Audit
{
    public interface IEventEntry
    {
        string Schema { get; set; }
        string Table { get; set; }
        string Name { get; set; }
        int Action { get; set; }
        List<IEventEntryChange> Changes { get; set; }
        IDictionary<string, object> ColumnValues { get; set; }
        bool Valid { get; set; }
        List<string> ValidationResults { get; set; }
        Type EntityType { get; set; }
        /// <summary>
        /// Returns the EntityEntry associated to this audit event entry
        /// </summary>
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry GetEntry();
    }
}
