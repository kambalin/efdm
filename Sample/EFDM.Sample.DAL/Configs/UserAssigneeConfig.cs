using EFDM.Sample.Core.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDM.Sample.DAL.Configs;

public class UserAssigneeConfig : IEntityTypeConfiguration<UserAssignee>
{
    public void Configure(EntityTypeBuilder<UserAssignee> builder)
    {
        builder.ToTable("UserAssignees");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("INT")
            .HasColumnOrder(0);

        builder.Property(x => x.UserId)
            .HasColumnType("INT")
            .HasColumnOrder(1);

        builder.Property(x => x.TypeId)
            .IsRequired()
            .HasColumnType("INT")
            .HasColumnOrder(2);

        builder.Property(x => x.ObjectId)
            .HasColumnType("INT")
            .HasColumnOrder(3);

        builder.Property(x => x.Active)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnType("BIT")
            .HasColumnOrder(4);

        builder.HasOne(x => x.User)
            .WithMany(u => u.UserAssignees)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(x => x.Type)
            .WithMany()
            .HasForeignKey(x => x.TypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.StartDate)
            .HasColumnType("DATETIMEOFFSET");

        builder.Property(x => x.EndDate)
            .HasColumnType("DATETIMEOFFSET");

        builder
            .OwnsOne(x => x.Data, nav => nav.ToJson());

        builder.Property(x => x.Created)
        .IsRequired()
        .HasDefaultValueSql("SYSDATETIMEOFFSET()")
        .HasColumnOrder(101);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.TypeId, x.ObjectId });
    }
}
