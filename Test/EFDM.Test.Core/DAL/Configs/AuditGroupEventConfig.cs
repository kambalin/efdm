using EFDM.Test.Core.Models.Audit;
using EFDM.Test.Core.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Test.Core.DAL.Configs {

    public class AuditGroupEventConfig : IEntityTypeConfiguration<AuditGroupEvent> {

        public void Configure(EntityTypeBuilder<AuditGroupEvent> builder) {
            builder.ToTable("AuditGroupEvents");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
