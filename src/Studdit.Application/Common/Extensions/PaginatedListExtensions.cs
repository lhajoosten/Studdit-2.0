using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Studdit.Application.Common.Models;

namespace Studdit.Application.Common.Extensions
{
    /// <summary>
    /// Extension methods for PaginatedList
    /// </summary>
    public static class PaginatedListExtensions
    {
        public static ActionResult<PaginatedList<T>> ToActionResult<T>(this PaginatedList<T> paginatedList, HttpContext context)
        {
            // Add pagination headers
            context.Response.Headers.Add("X-Pagination-TotalCount", paginatedList.TotalCount.ToString());
            context.Response.Headers.Add("X-Pagination-PageIndex", paginatedList.PageIndex.ToString());
            context.Response.Headers.Add("X-Pagination-PageSize", paginatedList.PageSize.ToString());
            context.Response.Headers.Add("X-Pagination-TotalPages", paginatedList.TotalPages.ToString());
            context.Response.Headers.Add("X-Pagination-HasPrevious", paginatedList.HasPreviousPage.ToString());
            context.Response.Headers.Add("X-Pagination-HasNext", paginatedList.HasNextPage.ToString());

            return new OkObjectResult(paginatedList);
        }
    }
}
