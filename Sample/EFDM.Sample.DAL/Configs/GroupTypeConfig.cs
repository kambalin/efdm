using EFDM.Sample.Core.Constants.ModelValues;
using EFDM.Sample.Core.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace EFDM.Sample.DAL.Configs;

public class GroupTypeConfig : IEntityTypeConfiguration<GroupType>
{
    public void Configure(EntityTypeBuilder<GroupType> builder)
    {
        builder.ToTable("GroupTypes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnOrder(0);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(150).HasColumnOrder(1);

        builder.HasData
        (
            new GroupType
            {
                Id = GroupTypeValues.Users,
                Title = GroupTypeValues.UsersTitle,
                Created = new DateTimeOffset(2024, 1, 1, 0, 0, 0, new TimeSpan(0, 3, 0, 0, 0)),
                Modified = new DateTimeOffset(2024, 1, 1, 0, 0, 0, new TimeSpan(0, 3, 0, 0, 0)),
                CreatedById = UserValues.SystemId,
                ModifiedById = UserValues.SystemId
            },
            new GroupType
            {
                Id = GroupTypeValues.Administrators,
                Title = GroupTypeValues.AdministratorsTitle,
                Created = new DateTimeOffset(2024, 1, 1, 0, 0, 0, new TimeSpan(0, 3, 0, 0, 0)),
                Modified = new DateTimeOffset(2024, 1, 1, 0, 0, 0, new TimeSpan(0, 3, 0, 0, 0)),
                CreatedById = UserValues.SystemId,
                ModifiedById = UserValues.SystemId
            }
        );
    }
}
