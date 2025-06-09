namespace Studdit.Application.Common.Models
{
    /// <summary>
    /// API response wrapper for consistent response format
    /// </summary>
    /// <typeparam name="T">The type of data being returned</typeparam>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }
        public string? Message { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static ApiResponse<T> ErrorResponse(string error)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Error = error
            };
        }
    }

    /// <summary>
    /// API response wrapper without data
    /// </summary>
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public string? Message { get; set; }

        public static ApiResponse SuccessResponse(string? message = null)
        {
            return new ApiResponse
            {
                Success = true,
                Message = message
            };
        }

        public static ApiResponse ErrorResponse(string error)
        {
            return new ApiResponse
            {
                Success = false,
                Error = error
            };
        }
    }
}
