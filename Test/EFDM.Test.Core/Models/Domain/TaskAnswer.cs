using EFDM.Core.Models.Domain;
using EFDM.Test.Core.Models.Domain.Base;
using System.Collections.Generic;

namespace EFDM.Test.Core.Models.Domain {

    public class TaskAnswer : IdKeyEntityBase<int> {

        public decimal AnswerValue { get; set; }
        public virtual TaskAnswerComment AnswerComment { get; set; }
    }
}
