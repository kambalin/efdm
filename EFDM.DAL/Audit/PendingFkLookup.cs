using EFDM.Core.Audit;
using System;

namespace EFDM.DAL.Audit
{
    /// <summary>
    /// FK change of an audit event entry whose display values (old/new) are pending resolution:
    /// collected while building the event, resolved in one query per related type.
    /// </summary>
    internal sealed class PendingFkLookup
    {
        public EventEntryChange Change { get; set; }
        public Type RelatedType { get; set; }
        public object OriginalId { get; set; }
        public object NewId { get; set; }
    }
}
