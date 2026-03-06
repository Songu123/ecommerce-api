using API.Models;
using API.Models.Enums;

namespace API.Repositories.Interfaces
{
    /// <summary>
    /// Order repository interface
    /// </summary>
    public interface IOrderRepository : IGenericRepository<Order>
    {
        /// <summary>
        /// Get order by order code
        /// </summary>
     Task<Order?> GetByOrderCodeAsync(string orderCode);

        /// <summary>
        /// Get orders by user ID
        /// </summary>
 Task<List<Order>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Get orders by status
      /// </summary>
   Task<List<Order>> GetByStatusAsync(OrderStatus status);

        /// <summary>
        /// Get order with items and product details
        /// </summary>
        Task<Order?> GetOrderWithDetailsAsync(int id);

    /// <summary>
  /// Get user orders with details
     /// </summary>
  Task<List<Order>> GetUserOrdersWithDetailsAsync(int userId);

        /// <summary>
        /// Update order status
        /// </summary>
        Task UpdateOrderStatusAsync(int orderId, OrderStatus status);

  /// <summary>
        /// Get orders with pagination and filters
        /// </summary>
        Task<(List<Order> Orders, int TotalCount)> GetOrdersPagedAsync(
            int page, 
     int pageSize,
            OrderStatus? status = null,
   int? userId = null,
   DateTime? fromDate = null,
      DateTime? toDate = null);
    }
}
