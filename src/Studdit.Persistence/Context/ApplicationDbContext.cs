using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Studdit.Application.Common.Interfaces;
using Studdit.Domain.Common;
using Studdit.Domain.Entities;
using Studdit.Persistence.Interceptors;
using System.Reflection;

namespace Studdit.Persistence.Context
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService,
            IDateTimeService dateTimeService) : base(options)
        {
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }

        // DbSets for all domain entities
        public DbSet<User> Users => Set<User>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<Answer> Answers => Set<Answer>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Vote> Votes => Set<Vote>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply all entity configurations from the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(new AuditableEntitySaveChangesInterceptor(_currentUserService, _dateTimeService));
            base.OnConfiguring(optionsBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditableEntitiesSync();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            UpdateAuditableEntitiesSync();
            return base.SaveChanges();
        }

        private void UpdateAuditableEntitiesSync()
        {
            var entries = ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    entityEntry.Property(nameof(BaseEntity.CreatedDate)).CurrentValue = _dateTimeService.Now;

                    if (_currentUserService.UserId.HasValue)
                    {
                        entityEntry.Property(nameof(BaseEntity.CreatedByUserId)).CurrentValue = _currentUserService.UserId.Value;
                    }
                }

                if (entityEntry.State == EntityState.Modified)
                {
                    entityEntry.Property(nameof(BaseEntity.LastModifiedDate)).CurrentValue = _dateTimeService.Now;

                    if (_currentUserService.UserId.HasValue)
                    {
                        entityEntry.Property(nameof(BaseEntity.LastModifiedByUserId)).CurrentValue = _currentUserService.UserId.Value;
                    }
                }
            }
        }
    }
}