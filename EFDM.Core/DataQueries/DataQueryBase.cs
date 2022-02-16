using EFDM.Abstractions.DataQueries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EFDM.Core.DataQueries {

    public abstract class DataQueryBase<TModel> : IDataQuery<TModel>
        where TModel : class {

        public IEnumerable<ISorting> Sorts { get; set; } = new List<Sort>();
        public IEnumerable<string> Includes { get; set; }
        public bool Tracking { get; set; } = false;
        public int Take { get; set; }
        public int Skip { get; set; }

        public virtual IQueryFilter<TModel> ToFilter() {
            var and = new QueryFilter<TModel>();
            return and;
        }

        protected IEnumerable<string> GetWords(string text) {
            return text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Regex.Replace(x, @"(\S)\+(\S)", "$1 $2"));
        }
    }
}
