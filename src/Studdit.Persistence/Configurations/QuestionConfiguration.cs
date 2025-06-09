using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studdit.Domain.Entities;

namespace Studdit.Persistence.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Questions");

            builder.HasKey(q => q.Id);

            builder.Property(q => q.Id)
                .ValueGeneratedOnAdd();

            builder.Property(q => q.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(q => q.Content)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(q => q.VoteScore)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(q => q.ViewCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(q => q.IsAnswered)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(q => q.IsClosed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(q => q.ClosedAt)
                .IsRequired(false);

            builder.Property(q => q.ClosureReason)
                .HasMaxLength(500);

            // Audit fields
            builder.Property(q => q.CreatedDate)
                .IsRequired();

            builder.Property(q => q.CreatedByUserId)
                .IsRequired(false);

            builder.Property(q => q.LastModifiedDate)
                .IsRequired(false);

            builder.Property(q => q.LastModifiedByUserId)
                .IsRequired(false);

            // Foreign key for Author
            builder.HasOne(q => q.Author)
                .WithMany(u => u.Questions)
                .HasForeignKey("AuthorId")
                .OnDelete(DeleteBehavior.Restrict);

            // Navigation properties
            builder.HasMany(q => q.Answers)
                .WithOne(a => a.Question)
                .HasForeignKey("QuestionId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(q => q.Votes)
                .WithOne(v => v.Question)
                .HasForeignKey("QuestionId")
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-many relationship with Tags
            builder.HasMany(q => q.Tags)
                .WithMany(t => t.Questions)
                .UsingEntity<Dictionary<string, object>>(
                    "QuestionTags",
                    j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                    j => j.HasOne<Question>().WithMany().HasForeignKey("QuestionId"),
                    j =>
                    {
                        j.HasKey("QuestionId", "TagId");
                        j.ToTable("QuestionTags");
                    });

            // Indexes for performance
            builder.HasIndex(q => q.CreatedDate);
            builder.HasIndex(q => q.VoteScore);
            builder.HasIndex(q => q.ViewCount);
            builder.HasIndex(q => q.IsAnswered);
            builder.HasIndex(q => q.IsClosed);
            builder.HasIndex("AuthorId");

            // Full-text search indexes (if using SQL Server)
            builder.HasIndex(q => q.Title);
            builder.HasIndex(q => q.Content);
        }
    }
}
