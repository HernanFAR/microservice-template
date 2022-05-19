using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Questions.Domain.DataAccess;
using Questions.Domain.Entities;

namespace Questions.EntityFramework.Relations
{
    internal class QuestionEntityConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable(nameof(Question), DatabaseConstants.Schema);

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .HasMaxLength(Question.NameMaxLength)
                .IsRequired();

            builder.Property(e => e.CreatedById)
                .IsRequired();

        }
    }
}
