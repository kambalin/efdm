using EFDM.Abstractions.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Abstractions.DataQueries {

    public interface IDataQuery<TModel, TKey> : IDataQuery<TModel>
        where TModel : class, IIdKeyEntity<TKey>
        where TKey : IComparable, IEquatable<TKey> {

        TKey[] Ids { get; set; }
        TKey[] NotIds { get; set; }
    }

    public interface IDataQuery<TModel> : IDataQuery {
        IQueryFilter<TModel> ToFilter();
    }

    public interface IDataQuery {
        int Take { get; set; }
        int Skip { get; set; }
        IEnumerable<ISorting> Sorts { get; set; }
        IEnumerable<string> Includes { get; set; }
        bool Tracking { get; set; }
    }
}
