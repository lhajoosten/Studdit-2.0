namespace Studdit.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for date/time service
    /// </summary>
    public interface IDateTimeService
    {
        DateTime Now { get; }
        DateTime Today { get; }
        DateTime TimeOfDay { get; }
    }
}
