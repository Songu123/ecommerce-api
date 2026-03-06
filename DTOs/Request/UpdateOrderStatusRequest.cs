using System.ComponentModel.DataAnnotations;
using API.Models.Enums;

namespace API.DTOs.Request
{
    /// <summary>
    /// DTO for updating order status
    /// </summary>
    public class UpdateOrderStatusRequest
    {
        [Required(ErrorMessage = "Tr?ng thái là b?t bu?c")]
        public OrderStatus Status { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
