using EFDM.Sample.Core.Constants.ModelValues;
using EFDM.Sample.Core.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;

namespace EFDM.Sample.DAL.Configs;

public class UserAssigneeTypeConfig : IEntityTypeConfiguration<UserAssigneeType>
{
    public void Configure(EntityTypeBuilder<UserAssigneeType> builder)
    {
        builder.ToTable("UserAssigneeTypes");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("INT")
            .HasColumnOrder(0);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnType("NVARCHAR(150)")
            .HasColumnOrder(1);

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnType("BIT")
            .HasColumnOrder(100);

        builder.Property(x => x.Created)
            .IsRequired()
            .HasDefaultValueSql("SYSDATETIMEOFFSET()")
            .HasColumnOrder(101);

        builder.Property(x => x.Modified)
            .IsRequired()
            .HasDefaultValueSql("SYSDATETIMEOFFSET()")
            .HasColumnOrder(102);

        builder.Property(x => x.CreatedById)
            .IsRequired()
            .HasDefaultValue(UserValues.SystemId)
            .HasColumnType("INT")
            .HasColumnOrder(103);

        builder.Property(x => x.ModifiedById)
            .IsRequired()
            .HasDefaultValue(UserValues.SystemId)
            .HasColumnType("INT")
            .HasColumnOrder(104);

        builder.HasOne(x => x.CreatedBy)
               .WithMany()
               .HasForeignKey(x => x.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ModifiedBy)
               .WithMany()
               .HasForeignKey(x => x.ModifiedById)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(new[]
            {
                new UserAssigneeType
                {
                    Id = 1,
                    Title = "Назначение 1"
                },
                new UserAssigneeType
                {
                    Id = 2,
                    Title = "Назначение 2"
                }
            }.Select(o =>
            {
                o.Created = new DateTimeOffset(2020, 1, 1, 0, 0, 0, new TimeSpan(0, 3, 0, 0, 0));
                o.Modified = new DateTimeOffset(2020, 1, 1, 0, 0, 0, new TimeSpan(0, 3, 0, 0, 0));
                return o;
            })
        );
    }
}
