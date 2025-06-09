using MediatR;
using Microsoft.Extensions.Logging;
using Studdit.Application.Common.Interfaces;
using System.Diagnostics;

namespace Studdit.Application.Common.Behaviours
{
    /// <summary>
    /// Pipeline behavior for logging performance
    /// </summary>
    /// <typeparam name="TRequest">The type of the request</typeparam>
    /// <typeparam name="TResponse">The type of the response</typeparam>
    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<TRequest> _logger;
        private readonly ICurrentUserService _currentUserService;

        public PerformanceBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var response = await next();

            stopwatch.Stop();

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;
                var userId = _currentUserService.UserId;
                var userName = _currentUserService.Username ?? string.Empty;

                _logger.LogWarning("Studdit Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}",
                    requestName, elapsedMilliseconds, userId, userName, request);
            }

            return response;
        }
    }
}
