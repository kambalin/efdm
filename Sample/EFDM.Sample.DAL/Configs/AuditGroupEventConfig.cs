using EFDM.Sample.Core.Models.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDM.Sample.DAL.Configs
{
    public class AuditGroupEventConfig : IEntityTypeConfiguration<AuditGroupEvent>
    {
        public void Configure(EntityTypeBuilder<AuditGroupEvent> builder)
        {
            builder.ToTable("AuditGroupEvents");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnOrder(0);
        }
    }
}
