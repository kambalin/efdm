using EFDM.Sample.Core.Models.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDM.Sample.DAL.Configs
{
    public class AuditableUserEntityConfig<T> : IEntityTypeConfiguration<T>
        where T : class, IAuditableUserEntity
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.ModifiedBy)
                .WithMany()
                .HasForeignKey(x => x.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
