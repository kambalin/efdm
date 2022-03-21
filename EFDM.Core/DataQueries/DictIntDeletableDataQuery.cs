using EFDM.Abstractions.Models.Domain;

namespace EFDM.Core.DataQueries {

    public class DictIntDeletableDataQuery<TModel> : DictDeletableDataQueryBase<TModel, int>
        where TModel : class, IDictDeletableEntityBase<int> {
    }
}
