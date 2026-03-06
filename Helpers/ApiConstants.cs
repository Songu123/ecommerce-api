namespace API.Helpers
{
    /// <summary>
    /// Constants cho toàn b? API
    /// </summary>
    public static class ApiConstants
    {
        // API Versions
   public const string ApiVersion = "v1";
        public const string ApiPrefix = "api";

        // Roles
      public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

      // Default Values
   public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;

      // Cache Keys
        public const string CategoriesCacheKey = "categories_all";
        public const string ProductsCacheKey = "products_page_{0}";

        // Error Messages
      public const string NotFoundMessage = "Resource not found";
        public const string UnauthorizedMessage = "Unauthorized access";
      public const string ValidationErrorMessage = "Validation failed";
        public const string ServerErrorMessage = "Internal server error";
    }

    /// <summary>
    /// Pagination parameters
    /// </summary>
    public class PaginationParams
    {
   private int _pageSize = ApiConstants.DefaultPageSize;

      public int PageNumber { get; set; } = 1;
        
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > ApiConstants.MaxPageSize 
            ? ApiConstants.MaxPageSize 
        : value;
        }
    }
}
