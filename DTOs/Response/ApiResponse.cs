namespace API.DTOs.Response
{
    /// <summary>
    /// Standard API Response wrapper
  /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
  public string Message { get; set; } = string.Empty;
 public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
     Success = true,
             Message = message,
                Data = data
 };
        }

  public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
         return new ApiResponse<T>
            {
    Success = false,
      Message = message,
     Errors = errors
            };
     }
    }

    /// <summary>
    /// Paginated response for list data
    /// </summary>
 /// <typeparam name="T">Type of items</typeparam>
    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
  public bool HasNext => CurrentPage < TotalPages;
  }
}
