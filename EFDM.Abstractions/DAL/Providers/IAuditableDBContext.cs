using EFDM.Abstractions.Audit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Abstractions.DAL.Providers {

    public interface IAuditableDBContext {
        IDBContextAuditor Auditor { get; }
        DbContext DbContext { get; }
        void InitAuditMapping();
    }
}
