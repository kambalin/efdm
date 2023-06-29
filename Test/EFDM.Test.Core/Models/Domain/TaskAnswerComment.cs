using EFDM.Core.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace EFDM.Test.Core.Models.Domain
{
    public class TaskAnswerComment : IdKeyEntityBase<int>
    {
        public string? Comment { get; set; }
        [Required]
        public virtual TaskAnswer TaskAnswer { get; set; }
    }
}
