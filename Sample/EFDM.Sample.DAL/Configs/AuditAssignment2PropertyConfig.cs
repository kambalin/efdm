using EFDM.Sample.Core.Models.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDM.Sample.DAL.Configs
{
    public class AuditAssignment2PropertyConfig : IEntityTypeConfiguration<AuditAssignment2Property>
    {
        public void Configure(EntityTypeBuilder<AuditAssignment2Property> builder)
        {
            builder.ToTable("AuditAssignment2Properties");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnOrder(0);
        }
    }
}
