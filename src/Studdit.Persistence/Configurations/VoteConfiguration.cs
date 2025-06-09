using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studdit.Domain.Entities;
using Studdit.Domain.Enums;

namespace Studdit.Persistence.Configurations
{
    public class VoteConfiguration : IEntityTypeConfiguration<Vote>
    {
        public void Configure(EntityTypeBuilder<Vote> builder)
        {
            builder.ToTable("Votes");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Id)
                .ValueGeneratedOnAdd();

            // VoteType enumeration configuration
            builder.Property(v => v.Type)
                .HasConversion(
                    voteType => voteType.Id,
                    value => VoteType.FromValue<VoteType>(value))
                .IsRequired();

            // Audit fields
            builder.Property(v => v.CreatedDate)
                .IsRequired();

            builder.Property(v => v.CreatedByUserId)
                .IsRequired(false);

            builder.Property(v => v.LastModifiedDate)
                .IsRequired(false);

            builder.Property(v => v.LastModifiedByUserId)
                .IsRequired(false);

            // Foreign keys
            builder.HasOne(v => v.User)
                .WithMany(u => u.Votes)
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.Question)
                .WithMany(q => q.Votes)
                .HasForeignKey("QuestionId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            builder.HasOne(v => v.Answer)
                .WithMany(a => a.Votes)
                .HasForeignKey("AnswerId")
                .OnDelete(DeleteBehavior.NoAction) // Prevent cascade delete conflicts
                .IsRequired(false);

            // Constraints
            // A user can only vote once per question
            builder.HasIndex("UserId", "QuestionId")
                .IsUnique()
                .HasFilter("QuestionId IS NOT NULL");

            // A user can only vote once per answer
            builder.HasIndex("UserId", "AnswerId")
                .IsUnique()
                .HasFilter("AnswerId IS NOT NULL");

            // Ensure vote is either for question or answer, not both
            builder.ToTable(t => t.HasCheckConstraint(
                "CK_Vote_QuestionOrAnswer",
                "(QuestionId IS NOT NULL AND AnswerId IS NULL) OR (QuestionId IS NULL AND AnswerId IS NOT NULL)"));

            // Indexes for performance
            builder.HasIndex("UserId");
            builder.HasIndex("QuestionId");
            builder.HasIndex("AnswerId");
            builder.HasIndex(v => v.Type);
            builder.HasIndex(v => v.LastModifiedDate);
        }
    }
}
