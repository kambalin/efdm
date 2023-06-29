using EFDM.Test.Core.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDM.Test.DAL.Configs
{
    public class TaskAnswerConfig : IEntityTypeConfiguration<TaskAnswer>
    {
        public void Configure(EntityTypeBuilder<TaskAnswer> builder)
        {
            builder.ToTable("TaskAnswers");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.HasOne(a => a.AnswerComment)
                .WithOne(b => b.TaskAnswer)
                .IsRequired()
                .HasForeignKey<TaskAnswerComment>(b => b.Id);
        }
    }
}
