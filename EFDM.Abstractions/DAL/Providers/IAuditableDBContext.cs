using EFDM.Abstractions.Audit;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EFDM.Abstractions.DAL.Providers
{
    public interface IAuditableDBContext
    {
        IDBContextAuditor Auditor { get; }
        DbContext DbContext { get; }
        void InitAuditMapping();
        /// <summary>
        /// Persist provided audit entities. Implementations should persist audit entities
        /// after the main SaveChanges has completed to avoid nested SaveChanges during ChangeTracker processing.
        /// </summary>
        /// <param name="entities">Audit entities to persist.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<int> PersistAuditEntriesAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default);
        int PersistAuditEntries(IEnumerable<object> entities);
    }
}
