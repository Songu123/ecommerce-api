using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    /// <summary>
    /// User entity
    /// </summary>
 public class User
    {
   public int Id { get; set; }

    [Required]
   [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

    [Required]
        [MaxLength(100)]
        [EmailAddress]
  public string Email { get; set; } = string.Empty;

        [Required]
     [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Role { get; set; } = "Customer"; // Customer, Admin

      public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
