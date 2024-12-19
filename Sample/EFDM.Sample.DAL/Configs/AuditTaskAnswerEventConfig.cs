using EFDM.Sample.Core.Models.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDM.Sample.DAL.Configs
{
    public class AuditTaskAnswerEventConfig : IEntityTypeConfiguration<AuditTaskAnswerEvent>
    {
        public void Configure(EntityTypeBuilder<AuditTaskAnswerEvent> builder)
        {
            builder.ToTable("AuditTaskAnswerEvents");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnOrder(0);
        }
    }
}
