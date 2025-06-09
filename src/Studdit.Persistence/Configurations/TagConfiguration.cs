using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studdit.Domain.Entities;

namespace Studdit.Persistence.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("Tags");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ValueGeneratedOnAdd();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(t => t.Name)
                .IsUnique();

            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.UsageCount)
                .IsRequired()
                .HasDefaultValue(0);

            // Audit fields
            builder.Property(t => t.CreatedDate)
                .IsRequired();

            builder.Property(t => t.CreatedByUserId)
                .IsRequired(false);

            builder.Property(t => t.LastModifiedDate)
                .IsRequired(false);

            builder.Property(t => t.LastModifiedByUserId)
                .IsRequired(false);

            // Many-to-many relationship with Questions (configured in QuestionConfiguration)
            builder.HasMany(t => t.Questions)
                .WithMany(q => q.Tags);

            // Indexes for performance
            builder.HasIndex(t => t.UsageCount);
            builder.HasIndex(t => t.CreatedAt);
        }
    }
}
