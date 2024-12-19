using EFDM.Sample.Core.Constants.ModelValues;
using EFDM.Sample.Core.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace EFDM.Sample.DAL.Configs;

public class UsersConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnOrder(0);
        builder.Property(x => x.Login).IsRequired().HasMaxLength(100).HasColumnOrder(1);
        builder.Property(x => x.Title).HasColumnOrder(2);

        //builder.HasOne(x => x.CreatedBy)
        //    .WithMany()
        //    .HasForeignKey(x => x.CreatedById)
        //    .OnDelete(DeleteBehavior.Restrict);
        //builder.HasOne(x => x.ModifiedBy)
        //    .WithMany()
        //    .HasForeignKey(x => x.ModifiedById)
        //    .OnDelete(DeleteBehavior.Restrict);

        builder.HasData
        (
            new User
            {
                Id = UserValues.SystemId,
                Title = UserValues.SystemTitle,
                Login = $"{UserValues.SystemDomain}\\{UserValues.SystemTitle}",
                Created = new DateTimeOffset(2023, 12, 1, 0, 0, 0, new TimeSpan(0, 3, 0, 0, 0)),
                Modified = new DateTimeOffset(2023, 12, 1, 0, 0, 0, new TimeSpan(0, 3, 0, 0, 0)),
                CreatedById = UserValues.SystemId,
                ModifiedById = UserValues.SystemId
            }
        );
    }
}
