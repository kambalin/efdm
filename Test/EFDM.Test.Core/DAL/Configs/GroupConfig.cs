using EFDM.Test.Core.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Test.Core.DAL.Configs {

    public class GroupConfig : IEntityTypeConfiguration<Group> {

        public void Configure(EntityTypeBuilder<Group> builder) {
            builder.ToTable("Groups");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
            builder.HasOne(x => x.Type)
                .WithMany(y => y.Groups)
                .HasForeignKey(x => x.TypeId);

            //builder.HasOne(x => x.CreatedBy)
            //    .WithMany()
            //    .HasForeignKey(x => x.CreatedById);
            //builder.HasOne(x => x.ModifiedBy)
            //    .WithMany()
            //    .HasForeignKey(x => x.ModifiedById);
        }
    }
}
