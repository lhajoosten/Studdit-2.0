using Studdit.Application.Common.Interfaces;

namespace Studdit.Application.Common.Services
{
    /// <summary>
    /// Service for date/time operations (useful for testing)
    /// </summary>
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.Now;
        public DateTime Today => DateTime.Today;
        public DateTime TimeOfDay => DateTime.Now.TimeOfDay > TimeSpan.Zero ? DateTime.Now : DateTime.Today;
    }
}
