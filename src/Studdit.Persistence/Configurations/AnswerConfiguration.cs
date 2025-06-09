using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studdit.Domain.Entities;

namespace Studdit.Persistence.Configurations
{
    public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.ToTable("Answers");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            builder.Property(a => a.Content)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.VoteScore)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(a => a.IsAccepted)
                .IsRequired()
                .HasDefaultValue(false);

            // Audit fields
            builder.Property(a => a.CreatedDate)
                .IsRequired();

            builder.Property(a => a.CreatedByUserId)
                .IsRequired(false);

            builder.Property(a => a.LastModifiedDate)
                .IsRequired(false);

            builder.Property(a => a.LastModifiedByUserId)
                .IsRequired(false);

            // Foreign keys
            builder.HasOne(a => a.Author)
                .WithMany(u => u.Answers)
                .HasForeignKey("AuthorId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey("QuestionId")
                .OnDelete(DeleteBehavior.Cascade);

            // Navigation properties
            builder.HasMany(a => a.Votes)
                .WithOne(v => v.Answer)
                .HasForeignKey("AnswerId")
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            builder.HasIndex(a => a.CreatedDate);
            builder.HasIndex(a => a.VoteScore);
            builder.HasIndex(a => a.IsAccepted);
            builder.HasIndex("QuestionId");
            builder.HasIndex("AuthorId");

            // Unique constraint: only one accepted answer per question per author
            builder.HasIndex("QuestionId", "IsAccepted")
                .HasFilter("IsAccepted = 1");
        }
    }
}
