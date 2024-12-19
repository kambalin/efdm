using EFDM.Abstractions.Services.Domain;
using EFDM.Sample.Core.DataQueries.Models;
using EFDM.Sample.Core.Models.Domain;

namespace EFDM.Sample.Core.Services.Domain.Interfaces
{
    public interface ITaskAnswerCommentService : IDomainService<TaskAnswerComment,
        TaskAnswerCommentQuery, int>
    {
    }
}
