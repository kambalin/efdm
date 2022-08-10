using EFDM.Test.Core.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDM.Test.DAL.Configs {

    public class TaskAnswerCommentsConfig : IEntityTypeConfiguration<TaskAnswerComment> {

        public void Configure(EntityTypeBuilder<TaskAnswerComment> builder) {
            builder.ToTable("TaskAnswerComments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
