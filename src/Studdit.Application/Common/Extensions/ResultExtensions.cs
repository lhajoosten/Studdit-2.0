using Microsoft.AspNetCore.Mvc;
using Studdit.Application.Common.Models;

namespace Studdit.Application.Common.Extensions
{
    /// <summary>
    /// Extension methods for Result pattern integration with ASP.NET Core
    /// </summary>
    public static class ResultExtensions
    {
        public static ActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess)
            {
                return new OkResult();
            }

            return new BadRequestObjectResult(new { error = result.Error });
        }

        public static ActionResult<T> ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(result.Value);
            }

            return new BadRequestObjectResult(new { error = result.Error });
        }

        public static ActionResult ToCreatedActionResult<T>(this Result<T> result, string actionName, object routeValues)
        {
            if (result.IsSuccess)
            {
                return new CreatedAtActionResult(actionName, null, routeValues, result.Value);
            }

            return new BadRequestObjectResult(new { error = result.Error });
        }
    }
}
