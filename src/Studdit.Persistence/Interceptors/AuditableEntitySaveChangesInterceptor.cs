using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Studdit.Application.Common.Interfaces;
using Studdit.Domain.Common;

namespace Studdit.Persistence.Interceptors
{
    public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;

        public AuditableEntitySaveChangesInterceptor(
            ICurrentUserService currentUserService,
            IDateTimeService dateTimeService)
        {
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateEntities(DbContext? context)
        {
            if (context == null)
                return;

            foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.GetType()
                        .GetProperty(nameof(BaseEntity.CreatedByUserId))
                        ?.SetValue(entry.Entity, _currentUserService.UserId);

                    entry.Entity.GetType()
                        .GetProperty(nameof(BaseEntity.CreatedDate))
                        ?.SetValue(entry.Entity, _dateTimeService.Now);
                }

                if (entry.State is EntityState.Added or EntityState.Modified)
                {
                    entry.Entity.GetType()
                        .GetProperty(nameof(BaseEntity.LastModifiedByUserId))
                        ?.SetValue(entry.Entity, _currentUserService.UserId);

                    entry.Entity.GetType()
                        .GetProperty(nameof(BaseEntity.LastModifiedDate))
                        ?.SetValue(entry.Entity, _dateTimeService.Now);
                }
            }
        }
    }
}
