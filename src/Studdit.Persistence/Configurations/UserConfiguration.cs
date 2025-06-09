using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studdit.Domain.Entities;
using Studdit.Domain.ValueObjects;

namespace Studdit.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(u => u.Username)
                .IsUnique();

            // Email value object configuration
            builder.Property(u => u.Email)
                .HasConversion(
                    email => email.Value,
                    value => Email.Create(value))
                .IsRequired()
                .HasMaxLength(320);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.DisplayName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Bio)
                .HasMaxLength(1000);

            builder.Property(u => u.Reputation)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(u => u.LastLoginDate)
                .IsRequired(false);

            // Audit fields
            builder.Property(u => u.CreatedDate)
                .IsRequired();

            builder.Property(u => u.CreatedByUserId)
                .IsRequired(false);

            builder.Property(u => u.LastModifiedDate)
                .IsRequired(false);

            builder.Property(u => u.LastModifiedByUserId)
                .IsRequired(false);

            // Navigation properties
            builder.HasMany(u => u.Questions)
                .WithOne(q => q.Author)
                .HasForeignKey("AuthorId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Answers)
                .WithOne(a => a.Author)
                .HasForeignKey("AuthorId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Votes)
                .WithOne(v => v.User)
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            builder.HasIndex(u => u.IsActive);
            builder.HasIndex(u => u.Reputation);
            builder.HasIndex(u => u.CreatedDate);
        }
    }
}