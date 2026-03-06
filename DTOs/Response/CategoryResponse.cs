namespace API.DTOs.Response
{
    /// <summary>
    /// Category response DTO
  /// </summary>
    public class CategoryResponse
    {
 public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
      public string? Description { get; set; }
        public int ProductCount { get; set; }
 public DateTime CreatedAt { get; set; }
    }
}
