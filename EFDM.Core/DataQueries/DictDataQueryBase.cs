using EFDM.Abstractions.DataQueries;
using EFDM.Abstractions.Models.Domain;
using System;

namespace EFDM.Core.DataQueries
{
    public class DictDataQueryBase<TModel, TKey> : EntityDataQueryBase<TModel, TKey>
        where TModel : class, IDictEntityBase<TKey>
        where TKey : IComparable, IEquatable<TKey>
    {
        public string Title { get; set; }
        public string TitleContains { get; set; }
        public string TitleSplitContains { get; set; }

        public override IQueryFilter<TModel> ToFilter()
        {
            var and = new QueryFilter<TModel>();

            if (!string.IsNullOrEmpty(Title))
                and.Add(x => x.Title.Equals(Title));

            if (!string.IsNullOrEmpty(TitleContains))
                and.Add(x => x.Title.Contains(TitleContains));

            if (!string.IsNullOrEmpty(TitleSplitContains))
            {
                foreach (var word in GetWords(TitleSplitContains))
                {
                    and.Add(x => x.Title.Contains(word));
                }
            }

            return base.ToFilter().Add(and);
        }
    }
}
