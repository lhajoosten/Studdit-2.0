namespace Studdit.Application.Common.Interfaces
{

    /// <summary>
    /// Interface for application context
    /// </summary>
    public interface IApplicationDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
