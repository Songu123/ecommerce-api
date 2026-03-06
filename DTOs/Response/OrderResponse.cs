using API.Models.Enums;

namespace API.DTOs.Response
{
    /// <summary>
    /// Order response DTO
    /// </summary>
    public class OrderResponse
    {
        public int Id { get; set; }
    public string OrderCode { get; set; } = string.Empty;
  public int UserId { get; set; }
      public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
      public DateTime OrderDate { get; set; }
  public OrderStatus Status { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
        public string? ShippingNote { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemResponse> Items { get; set; } = new();
   public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Order item response DTO
    /// </summary>
    public class OrderItemResponse
  {
     public int Id { get; set; }
        public int ProductId { get; set; }
 public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
   public decimal TotalPrice { get; set; }
    }

    /// <summary>
    /// Order summary response (for list view)
    /// </summary>
    public class OrderSummaryResponse
    {
   public int Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
      public DateTime OrderDate { get; set; }
   public OrderStatus Status { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    public int ItemsCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Order statistics response
    /// </summary>
    public class OrderStatisticsResponse
 {
     public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
     public int ConfirmedOrders { get; set; }
        public int ShippingOrders { get; set; }
   public int CompletedOrders { get; set; }
  public int CancelledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
    public Dictionary<string, int> OrdersByStatus { get; set; } = new();
}
}
