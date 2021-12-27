using EFDM.Test.Core.Models.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDM.Test.DAL.Configs {

    public class AuditableUserEntityConfig<T> : IEntityTypeConfiguration<T>
        where T : class, IAuditableUserEntity {

        public void Configure(EntityTypeBuilder<T> builder) {
            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById);
            builder.HasOne(x => x.ModifiedBy)
                .WithMany()
                .HasForeignKey(x => x.ModifiedById);
        }
    }
}
