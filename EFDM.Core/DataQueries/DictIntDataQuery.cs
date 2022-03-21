using EFDM.Abstractions.Models.Domain;

namespace EFDM.Core.DataQueries {

    public class DictIntDataQuery<TModel> : DictDataQueryBase<TModel, int>
        where TModel : class, IDictEntityBase<int> {
    }
}
