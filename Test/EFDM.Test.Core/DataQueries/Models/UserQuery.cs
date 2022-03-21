using EFDM.Abstractions.DataQueries;
using EFDM.Core.DataQueries;
using EFDM.Test.Core.Models.Domain;
using System;
using System.Linq;

namespace EFDM.Test.Core.DataQueries.Models {

    public class UserQuery : DictIntDeletableDataQuery<User> {
        public string[] Logins { get; set; }
        public string Text { get; set; }
        public int? GroupId { get; set; }

        public override IQueryFilter<User> ToFilter() {
            var and = new QueryFilter<User>();

            if (Logins?.Any() == true) {
                var lcLogins = Logins.Select(x => x.ToLower()).ToArray();
                and.Add(x => lcLogins.Contains(x.Login.ToLower()));
            }

            if (!string.IsNullOrEmpty(Text))
                foreach (var word in GetWords(Text))
                    and.Or(or => or
                        .Add(x => x.Login.Contains(word))
                        .Add(x => x.Title.Contains(word))
                        .Add(x => x.Email.Contains(word))
                    );

            if (GroupId.HasValue)
                and.Add(x => x.Groups.Any(g => g.GroupId == GroupId));

            return base.ToFilter().Add(and);
        }
    }
}
