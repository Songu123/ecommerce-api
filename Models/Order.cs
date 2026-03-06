using System.ComponentModel.DataAnnotations;
using API.Models.Enums;

namespace API.Models
{
    /// <summary>
    /// Order entity
    /// </summary>
    public class Order
    {
    public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string OrderCode { get; set; } = string.Empty;

      [Required]
        public int UserId { get; set; }
     
     public User? User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

  [Required]
        [MaxLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? ShippingNote { get; set; }

        [Required]
        [MaxLength(100)]
     public string PaymentMethod { get; set; } = string.Empty;

        public bool IsPaid { get; set; } = false;

        [Required]
     public decimal TotalAmount { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
