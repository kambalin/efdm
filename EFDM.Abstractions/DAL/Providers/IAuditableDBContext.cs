using EFDM.Abstractions.Audit;
using Microsoft.EntityFrameworkCore;

namespace EFDM.Abstractions.DAL.Providers
{
    public interface IAuditableDBContext
    {
        IDBContextAuditor Auditor { get; }
        DbContext DbContext { get; }
        void InitAuditMapping();
    }
}
