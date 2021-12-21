using EFDM.Abstractions.Models.Domain;
using EFDM.Core.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Core.DataQueries {

    public class DictIntDeletableDataQuery<TModel> : DictDeletableDataQueryBase<TModel, int>
        where TModel : class, IDictDeletableEntityBase<int> { 
    }
}
