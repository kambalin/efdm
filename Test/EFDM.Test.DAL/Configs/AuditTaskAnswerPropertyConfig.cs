using EFDM.Test.Core.Models.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDM.Test.DAL.Configs {

    public class AuditTaskAnswerPropertyConfig : IEntityTypeConfiguration<AuditTaskAnswerProperty> {

        public void Configure(EntityTypeBuilder<AuditTaskAnswerProperty> builder) {
            builder.ToTable("AuditTaskAnswerProperties");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
