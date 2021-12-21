using EFDM.Abstractions.DataQueries;
using EFDM.Core.DataQueries;
using EFDM.Test.Core.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Test.Core.DataQueries.Models {

    public class GroupQuery : DictIntDeletableDataQuery<Group> {
        public int[] UserIds { get; set; }
        public int[] TypeIds { get; set; }

        public override IQueryFilter<Group> ToFilter() {
            var and = new QueryFilter<Group>();

            if (UserIds?.Any() == true)
                and.Add(x => x.Users.Any(xx => UserIds.Contains(xx.UserId)));

            if (TypeIds?.Any() == true)
                and.Add(x => TypeIds.Contains(x.TypeId));

            return base.ToFilter().Add(and);
        }
    }
}
