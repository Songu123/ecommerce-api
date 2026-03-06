using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    /// <summary>
    /// Product entity
    /// </summary>
    public class Product
    {
        public int Id { get; set; }

    [Required]
        [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [MaxLength(500)]
     public string? Description { get; set; }

      public string? ImageUrl { get; set; }

        public int Stock { get; set; } = 0;

        [Required]
        public int CategoryId { get; set; }
        
 public Category? Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
