using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    /// <summary>
    /// Order item entity (OrderDetail)
    /// </summary>
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        public Order? Order { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
