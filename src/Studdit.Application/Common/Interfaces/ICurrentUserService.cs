namespace Studdit.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for the current user service
    /// </summary>
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? Username { get; }
        bool IsAuthenticated { get; }
    }
}
