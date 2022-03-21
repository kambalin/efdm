using EFDM.Abstractions.Audit;

namespace EFDM.Core.Audit {

    public class EventEntryChange : IEventEntryChange {
        public string ColumnName { get; set; }
        public object OriginalValue { get; set; }
        public object NewValue { get; set; }
    }
}
