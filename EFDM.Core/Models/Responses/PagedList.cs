using EFDM.Abstractions.Models.Responses;
using System.Collections.Generic;

namespace EFDM.Core.Models.Responses {

    public class PagedList<T> : IPagedList<T> {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Skipped { get; set; }
    }
}
