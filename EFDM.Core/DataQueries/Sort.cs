using EFDM.Abstractions.DataQueries;

namespace EFDM.Core.DataQueries
{
    public class Sort : ISorting
    {
        public bool Desc { get; set; }
        public string Field { get; set; }
    }
}
