using EFDM.Test.Core.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDM.Test.DAL.Configs {

    public class GroupTypeConfig : IEntityTypeConfiguration<GroupType> {

        public void Configure(EntityTypeBuilder<GroupType> builder) {
            builder.ToTable("GroupTypes");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
        }
    }
}
