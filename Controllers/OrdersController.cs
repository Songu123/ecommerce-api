using API.DTOs.Request;
using API.DTOs.Response;
using API.Models.Enums;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    /// <summary>
    /// Orders API Controller
    /// </summary>
    public class OrdersController : BaseApiController
    {
    private readonly IOrderService _orderService;
      private readonly ILogger<OrdersController> _logger;

  public OrdersController(
     IOrderService orderService,
 ILogger<OrdersController> logger)
     {
            _orderService = orderService;
      _logger = logger;
        }

        /// <summary>
     /// Get all orders (Admin only)
        /// </summary>
   /// <param name="page">Page number</param>
      /// <param name="pageSize">Page size</param>
        /// <param name="status">Filter by status</param>
        /// <param name="userId">Filter by user ID</param>
        /// <param name="fromDate">Filter from date</param>
        /// <param name="toDate">Filter to date</param>
  /// <returns>Paginated list of orders</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders(
  [FromQuery] int page = 1,
  [FromQuery] int pageSize = 10,
            [FromQuery] OrderStatus? status = null,
   [FromQuery] int? userId = null,
     [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
    {
   var result = await _orderService.GetOrdersAsync(page, pageSize, status, userId, fromDate, toDate);
        return PaginatedResponse(result);
        }

   /// <summary>
        /// Get current user's orders
        /// </summary>
   /// <returns>List of user's orders</returns>
      [HttpGet("my-orders")]
    [Authorize]
 public async Task<IActionResult> GetMyOrders()
     {
   var userId = GetCurrentUserId();
     var orders = await _orderService.GetUserOrdersAsync(userId);
   return SuccessResponse(orders, "L?y danh sách ??n hàng thành công");
 }

        /// <summary>
        /// Get order by ID
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order details</returns>
        [HttpGet("{id}")]
   [Authorize]
        public async Task<IActionResult> GetOrderById(int id)
   {
var order = await _orderService.GetOrderByIdAsync(id);
     if (order == null)
      return NotFoundResponse("??n hàng không t?n t?i");

     // Check if user owns the order or is admin
            var userId = GetCurrentUserId();
   var userRole = GetCurrentUserRole();

       if (order.UserId != userId && userRole != "Admin")
    return ErrorResponse("B?n không có quy?n xem ??n hàng này");

         return SuccessResponse(order, "L?y thông tin ??n hàng thành công");
        }

/// <summary>
        /// Get order by order code
        /// </summary>
    /// <param name="orderCode">Order code</param>
        /// <returns>Order details</returns>
        [HttpGet("code/{orderCode}")]
   [Authorize]
public async Task<IActionResult> GetOrderByCode(string orderCode)
        {
          var order = await _orderService.GetOrderByCodeAsync(orderCode);
            if (order == null)
       return NotFoundResponse("??n hàng không t?n t?i");

   // Check if user owns the order or is admin
    var userId = GetCurrentUserId();
      var userRole = GetCurrentUserRole();

       if (order.UserId != userId && userRole != "Admin")
          return ErrorResponse("B?n không có quy?n xem ??n hàng này");

            return SuccessResponse(order, "L?y thông tin ??n hàng thành công");
  }

   /// <summary>
        /// Create new order
        /// </summary>
/// <param name="request">Order data</param>
        /// <returns>Created order</returns>
        [HttpPost]
    [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
 {
            if (!ModelState.IsValid)
    return ErrorResponse("D? li?u không h?p l?", ModelState.Values
    .SelectMany(v => v.Errors)
         .Select(e => e.ErrorMessage)
       .ToList());

   try
      {
      var userId = GetCurrentUserId();
      var order = await _orderService.CreateOrderAsync(userId, request);
   return SuccessResponse(order, "T?o ??n hàng thành công");
   }
     catch (InvalidOperationException ex)
   {
       return ErrorResponse(ex.Message);
  }
        }

        /// <summary>
        /// Update order status (Admin only)
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="request">Status update request</param>
        /// <returns>Updated order</returns>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
   {
     if (!ModelState.IsValid)
    return ErrorResponse("D? li?u không h?p l?", ModelState.Values
  .SelectMany(v => v.Errors)
    .Select(e => e.ErrorMessage)
    .ToList());

       try
       {
     var order = await _orderService.UpdateOrderStatusAsync(id, request);
       return SuccessResponse(order, "C?p nh?t tr?ng thái ??n hàng thành công");
  }
       catch (InvalidOperationException ex)
  {
return ErrorResponse(ex.Message);
   }
  }

        /// <summary>
   /// Cancel order (Customer can cancel own order)
      /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Success result</returns>
   [HttpPost("{id}/cancel")]
        [Authorize]
public async Task<IActionResult> CancelOrder(int id)
   {
            try
  {
       var userId = GetCurrentUserId();
   var result = await _orderService.CancelOrderAsync(id, userId);

 if (!result)
     return NotFoundResponse("??n hàng không t?n t?i");

    return SuccessResponse(true, "H?y ??n hàng thành công");
  }
        catch (InvalidOperationException ex)
{
    return ErrorResponse(ex.Message);
  }
     catch (UnauthorizedAccessException ex)
   {
 return ErrorResponse(ex.Message);
   }
}

        /// <summary>
        /// Get order statistics (Admin only)
/// </summary>
        /// <param name="fromDate">From date</param>
        /// <param name="toDate">To date</param>
        /// <returns>Order statistics</returns>
        [HttpGet("statistics")]
  [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStatistics(
 [FromQuery] DateTime? fromDate = null,
       [FromQuery] DateTime? toDate = null)
        {
  var statistics = await _orderService.GetOrderStatisticsAsync(fromDate, toDate);
       return SuccessResponse(statistics, "L?y th?ng kê ??n hàng thành công");
    }

        #region Helper Methods

        private int GetCurrentUserId()
        {
   var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
     if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
     throw new UnauthorizedAccessException("Không th? xác th?c ng??i dùng");

            return userId;
        }

      private string GetCurrentUserRole()
   {
     return User.FindFirst(ClaimTypes.Role)?.Value ?? "Customer";
        }

        #endregion
    }
}
