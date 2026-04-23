using EFDM.Sample.Core.Models.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDM.Sample.DAL.Configs
{
    public class AuditAssignment1PropertyConfig : IEntityTypeConfiguration<AuditAssignment1Property>
    {
        public void Configure(EntityTypeBuilder<AuditAssignment1Property> builder)
        {
            builder.ToTable("AuditAssignment1Properties");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnOrder(0);
        }
    }
}
