namespace API.DTOs.Response
{
    /// <summary>
    /// Product response DTO
    /// </summary>
    public class ProductResponse
    {
        public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
 public int Stock { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
