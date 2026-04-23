using EFDM.Sample.Core.Models.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDM.Sample.DAL.Configs
{
    public class AuditAssignment2EventConfig : IEntityTypeConfiguration<AuditAssignment2Event>
    {
        public void Configure(EntityTypeBuilder<AuditAssignment2Event> builder)
        {
            builder.ToTable("AuditAssignment2Events");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnOrder(0);
        }
    }
}
