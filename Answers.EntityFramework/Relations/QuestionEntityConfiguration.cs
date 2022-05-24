using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Answers.Domain.DataAccess;
using Answers.Domain.Entities;

namespace Answers.EntityFramework.Relations
{
    internal class AnswerEntityConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.ToTable(nameof(Answer), DatabaseConstants.Schema);

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .HasMaxLength(Answer.NameMaxLength)
                .IsRequired();

            builder.Property(e => e.CreatedById)
                .IsRequired();

        }
    }
}
