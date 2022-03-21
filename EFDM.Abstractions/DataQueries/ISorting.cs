namespace EFDM.Abstractions.DataQueries {

    public interface ISorting {
        bool Desc { get; set; }
        string Field { get; set; }
    }
}
