using EFDM.Core.Models.Domain;
using System;

namespace EFDM.Test.Core.Models.Domain
{
    public class TaskAnswer : IdKeyEntityBase<int>
    {
        public decimal AnswerValue { get; set; }
        public virtual TaskAnswerComment AnswerComment { get; set; }
        public string TextField1 { get; set; }
        public string TextField2 { get; set; }
        public DateTimeOffset? ValidFrom { get; set; }
    }
}
