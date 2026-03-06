using System.ComponentModel.DataAnnotations;

namespace API.Models
{
 /// <summary>
  /// Category entity
    /// </summary>
    public class Category
    {
        public int Id { get; set; }

        [Required]
 [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
