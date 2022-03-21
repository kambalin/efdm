namespace EFDM.Abstractions.Audit {

    public interface IEventEntryChange {
        string ColumnName { get; set; }
        object OriginalValue { get; set; }
        object NewValue { get; set; }
    }
}
