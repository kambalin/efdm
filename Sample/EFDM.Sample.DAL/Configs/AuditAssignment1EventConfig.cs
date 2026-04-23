using EFDM.Sample.Core.Models.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDM.Sample.DAL.Configs
{
    public class AuditAssignment1EventConfig : IEntityTypeConfiguration<AuditAssignment1Event>
    {
        public void Configure(EntityTypeBuilder<AuditAssignment1Event> builder)
        {
            builder.ToTable("AuditAssignment1Events");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnOrder(0);
        }
    }
}
