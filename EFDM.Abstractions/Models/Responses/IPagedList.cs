using System.Collections.Generic;

namespace EFDM.Abstractions.Models.Responses {

    public interface IPagedList<T> {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int Skipped { get; set; }
    }
}
