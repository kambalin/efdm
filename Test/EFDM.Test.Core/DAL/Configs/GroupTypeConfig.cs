using EFDM.Test.Core.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Test.Core.DAL.Configs {

    public class GroupTypeConfig : IEntityTypeConfiguration<GroupType> {

        public void Configure(EntityTypeBuilder<GroupType> builder) {
            builder.ToTable("GroupTypes");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
        }
    }
}
