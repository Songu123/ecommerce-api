using API.DTOs.Request;
using API.DTOs.Response;
using API.Models.Enums;

namespace API.Services.Interfaces
{
    /// <summary>
    /// Order service interface
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Create new order
        /// </summary>
        Task<OrderResponse> CreateOrderAsync(int userId, CreateOrderRequest request);

        /// <summary>
        /// Get order by ID
   /// </summary>
  Task<OrderResponse?> GetOrderByIdAsync(int id);

  /// <summary>
        /// Get order by order code
        /// </summary>
        Task<OrderResponse?> GetOrderByCodeAsync(string orderCode);

   /// <summary>
 /// Get all orders with pagination
        /// </summary>
   Task<PaginatedResponse<OrderSummaryResponse>> GetOrdersAsync(
      int page,
            int pageSize,
  OrderStatus? status = null,
     int? userId = null,
   DateTime? fromDate = null,
DateTime? toDate = null);

        /// <summary>
    /// Get user's orders
 /// </summary>
Task<List<OrderResponse>> GetUserOrdersAsync(int userId);

        /// <summary>
     /// Update order status
     /// </summary>
      Task<OrderResponse> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request);

        /// <summary>
        /// Cancel order
        /// </summary>
   Task<bool> CancelOrderAsync(int orderId, int userId);

/// <summary>
        /// Get order statistics
        /// </summary>
   Task<OrderStatisticsResponse> GetOrderStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null);
    }
}
