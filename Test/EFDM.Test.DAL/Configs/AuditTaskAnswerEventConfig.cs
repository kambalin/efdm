using EFDM.Test.Core.Models.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDM.Test.DAL.Configs
{
    public class AuditTaskAnswerEventConfig : IEntityTypeConfiguration<AuditTaskAnswerEvent>
    {
        public void Configure(EntityTypeBuilder<AuditTaskAnswerEvent> builder)
        {
            builder.ToTable("AuditTaskAnswerEvents");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
