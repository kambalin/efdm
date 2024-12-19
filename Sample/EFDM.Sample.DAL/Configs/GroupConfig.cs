using EFDM.Sample.Core.Constants.ModelValues;
using EFDM.Sample.Core.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace EFDM.Sample.DAL.Configs;

public class GroupConfig : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("Groups");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnOrder(0);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(150).HasColumnOrder(1);
        builder.Property(x => x.TypeId).HasColumnOrder(2);
        builder.HasOne(x => x.Type)
            .WithMany(y => y.Groups)
            .HasForeignKey(x => x.TypeId);
        builder.Property(x => x.TextField1).HasMaxLength(150);
        builder.Property(x => x.TextField2).HasMaxLength(150);
        builder.Property(x => x.SubTypeId).IsRequired();

        builder.HasData
        (
            new Group
            {
                Id = GroupValues.Users,
                Title = GroupValues.UsersTitle,
                TypeId = GroupTypeValues.Users,
                Created = new DateTimeOffset(2024, 12, 1, 0, 0, 0, new TimeSpan(0, 3, 0, 0, 0)),
                Modified = new DateTimeOffset(2024, 12, 1, 0, 0, 0, new TimeSpan(0, 3, 0, 0, 0)),
                CreatedById = UserValues.SystemId,
                ModifiedById = UserValues.SystemId
            },
            new Group
            {
                Id = GroupValues.Administrators,
                Title = GroupValues.AdministratorsTitle,
                TypeId = GroupTypeValues.Administrators,
                Created = new DateTimeOffset(2024, 12, 1, 0, 0, 0, new TimeSpan(0, 3, 0, 0, 0)),
                Modified = new DateTimeOffset(2024, 12, 1, 0, 0, 0, new TimeSpan(0, 3, 0, 0, 0)),
                CreatedById = UserValues.SystemId,
                ModifiedById = UserValues.SystemId
            }
        );
    }
}
