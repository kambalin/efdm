﻿using EFDM.Abstractions.Services.Domain;
using EFDM.Test.Core.DataQueries.Models;
using EFDM.Test.Core.Models.Domain;

namespace EFDM.Test.Core.Services.Domain.Interfaces
{
    public interface ITaskAnswerCommentService : IDomainService<TaskAnswerComment,
        TaskAnswerCommentQuery, int>
    {
    }
}
