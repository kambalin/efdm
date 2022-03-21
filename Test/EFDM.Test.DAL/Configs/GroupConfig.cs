using EFDM.Test.Core.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDM.Test.DAL.Configs {

    public class GroupConfig : IEntityTypeConfiguration<Group> {

        public void Configure(EntityTypeBuilder<Group> builder) {
            builder.ToTable("Groups");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
            builder.HasOne(x => x.Type)
                .WithMany(y => y.Groups)
                .HasForeignKey(x => x.TypeId);
            builder.Property(x => x.TextField1).HasMaxLength(150);
            builder.Property(x => x.TextField2).HasMaxLength(150);
        }
    }
}
