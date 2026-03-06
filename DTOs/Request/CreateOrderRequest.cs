using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Request
{
    /// <summary>
    /// DTO for creating an order
    /// </summary>
    public class CreateOrderRequest
  {
  [Required(ErrorMessage = "??a ch? giao hàng là b?t bu?c")]
    [MaxLength(500, ErrorMessage = "??a ch? không ???c v??t quá 500 ký t?")]
        public string ShippingAddress { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Ghi chú không ???c v??t quá 200 ký t?")]
        public string? ShippingNote { get; set; }

        [Required(ErrorMessage = "Ph??ng th?c thanh toán là b?t bu?c")]
 [MaxLength(100)]
        public string PaymentMethod { get; set; } = string.Empty;

[Required(ErrorMessage = "Danh sách s?n ph?m không ???c ?? tr?ng")]
 [MinLength(1, ErrorMessage = "Ph?i có ít nh?t 1 s?n ph?m")]
      public List<OrderItemRequest> Items { get; set; } = new();
    }

    /// <summary>
    /// Order item request
    /// </summary>
    public class OrderItemRequest
 {
        [Required(ErrorMessage = "ProductId là b?t bu?c")]
        [Range(1, int.MaxValue, ErrorMessage = "ProductId ph?i l?n h?n 0")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "S? l??ng là b?t bu?c")]
    [Range(1, int.MaxValue, ErrorMessage = "S? l??ng ph?i l?n h?n 0")]
        public int Quantity { get; set; }
  }
}
