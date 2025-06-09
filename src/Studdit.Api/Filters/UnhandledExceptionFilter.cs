using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;


namespace Studdit.Api.Filters
{
    /// <summary>
    /// Catches unhandled exceptions and returns a standardized 500 error response.
    /// </summary>
    public class UnhandeledExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "An unexpected error occurred.",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                Detail = "The server encountered an unexpected condition that prevented it from fulfilling the request."
            };

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            context.ExceptionHandled = true;
        }
    }
}