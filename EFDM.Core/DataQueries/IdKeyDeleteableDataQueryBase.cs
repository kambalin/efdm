using EFDM.Abstractions.DataQueries;
using EFDM.Core.Models.Domain;
using System;

namespace EFDM.Core.DataQueries {

    public class IdKeyDeleteableDataQueryBase<TModel, TKey> : IdKeyDataQueryBase<TModel, TKey>
        where TModel : IdKeyDeletableEntityBase<TKey>
        where TKey : IComparable, IEquatable<TKey> {

        public bool? IsDeleted { get; set; }

        public override IQueryFilter<TModel> ToFilter() {
            var and = new QueryFilter<TModel>();

            if (IsDeleted != null)
                and.Add(x => x.IsDeleted == IsDeleted);

            return base.ToFilter().Add(and);
        }
    }
}
