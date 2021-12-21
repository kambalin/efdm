using EFDM.Test.Core.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Test.Core.DAL.Configs {

    public class GroupUserConfig : IEntityTypeConfiguration<GroupUser> {

        public void Configure(EntityTypeBuilder<GroupUser> builder) {
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
}
