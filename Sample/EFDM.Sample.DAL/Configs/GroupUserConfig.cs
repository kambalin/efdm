using EFDM.Sample.Core.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDM.Sample.DAL.Configs;

public class GroupUserConfig : IEntityTypeConfiguration<GroupUser>
{
    public void Configure(EntityTypeBuilder<GroupUser> builder)
    {
        builder.ToTable("GroupUsers");
        builder.HasKey(pc => new { pc.GroupId, pc.UserId });
        builder.HasOne(x => x.User)
            .WithMany(x => x.Groups)
            .HasForeignKey(x => x.UserId);
        builder.HasOne(pc => pc.Group)
            .WithMany(c => c.Users)
            .HasForeignKey(pc => pc.GroupId);
    }
}
