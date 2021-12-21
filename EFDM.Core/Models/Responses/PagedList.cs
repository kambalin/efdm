using EFDM.Abstractions.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Core.Models.Responses {

    public class PagedList<T> : IPagedList<T> {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Skipped { get; set; }
    }
}
