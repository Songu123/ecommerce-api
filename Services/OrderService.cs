using API.DTOs.Request;
using API.DTOs.Response;
using API.Models;
using API.Models.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    /// <summary>
    /// Order service implementation
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
     private readonly IProductRepository _productRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepo,
            IProductRepository productRepo,
   IUserRepository userRepo,
    ILogger<OrderService> logger)
        {
 _orderRepo = orderRepo;
            _productRepo = productRepo;
  _userRepo = userRepo;
 _logger = logger;
        }

        public async Task<OrderResponse> CreateOrderAsync(int userId, CreateOrderRequest request)
        {
        // Validate user exists
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
     throw new InvalidOperationException("Ng??i dùng không t?n t?i");

       // Validate products and calculate total
 var orderItems = new List<OrderItem>();
      decimal totalAmount = 0;

            foreach (var item in request.Items)
         {
           var product = await _productRepo.GetByIdAsync(item.ProductId);
 if (product == null)
           throw new InvalidOperationException($"S?n ph?m v?i ID {item.ProductId} không t?n t?i");

                if (product.Stock < item.Quantity)
        throw new InvalidOperationException($"S?n ph?m '{product.Name}' không ?? s? l??ng trong kho");

        var orderItem = new OrderItem
                {
     ProductId = product.Id,
   Quantity = item.Quantity,
        UnitPrice = product.Price,
    CreatedAt = DateTime.UtcNow
           };

       orderItems.Add(orderItem);
  totalAmount += orderItem.TotalPrice;

  // Update product stock
 product.Stock -= item.Quantity;
     product.UpdatedAt = DateTime.UtcNow;
            }

          // Generate order code
 var orderCode = await GenerateOrderCodeAsync();

  // Create order
    var order = new Order
            {
  OrderCode = orderCode,
  UserId = userId,
           OrderDate = DateTime.UtcNow,
      Status = OrderStatus.Pending,
       ShippingAddress = request.ShippingAddress,
        ShippingNote = request.ShippingNote,
      PaymentMethod = request.PaymentMethod,
     IsPaid = false,
                TotalAmount = totalAmount,
           OrderItems = orderItems,
        CreatedAt = DateTime.UtcNow
            };

 await _orderRepo.AddAsync(order);
            await _orderRepo.SaveAsync();

            _logger.LogInformation($"Order {orderCode} created successfully for user {userId}");

return await MapToOrderResponseAsync(order);
     }

        public async Task<OrderResponse?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepo.GetOrderWithDetailsAsync(id);
        if (order == null)
 return null;

    return await MapToOrderResponseAsync(order);
  }

        public async Task<OrderResponse?> GetOrderByCodeAsync(string orderCode)
        {
      var order = await _orderRepo.GetByOrderCodeAsync(orderCode);
            if (order == null)
             return null;

          return await MapToOrderResponseAsync(order);
   }

        public async Task<PaginatedResponse<OrderSummaryResponse>> GetOrdersAsync(
   int page,
            int pageSize,
   OrderStatus? status = null,
 int? userId = null,
          DateTime? fromDate = null,
         DateTime? toDate = null)
    {
            var (orders, totalCount) = await _orderRepo.GetOrdersPagedAsync(
       page, pageSize, status, userId, fromDate, toDate);

         var orderSummaries = orders.Select(o => new OrderSummaryResponse
        {
        Id = o.Id,
         OrderCode = o.OrderCode,
        CustomerName = o.User?.FullName ?? "N/A",
                OrderDate = o.OrderDate,
 Status = o.Status,
            StatusDisplay = GetStatusDisplay(o.Status),
       TotalAmount = o.TotalAmount,
 ItemsCount = o.OrderItems?.Count ?? 0,
          CreatedAt = o.CreatedAt
   }).ToList();

 return new PaginatedResponse<OrderSummaryResponse>
    {
  Items = orderSummaries,
 CurrentPage = page,
          TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
   PageSize = pageSize,
        TotalCount = totalCount
            };
   }

        public async Task<List<OrderResponse>> GetUserOrdersAsync(int userId)
        {
       var orders = await _orderRepo.GetUserOrdersWithDetailsAsync(userId);
         var orderResponses = new List<OrderResponse>();

foreach (var order in orders)
      {
        orderResponses.Add(await MapToOrderResponseAsync(order));
            }

   return orderResponses;
    }

        public async Task<OrderResponse> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request)
{
       var order = await _orderRepo.GetOrderWithDetailsAsync(orderId);
        if (order == null)
         throw new InvalidOperationException("??n hàng không t?n t?i");

            // Validate status transition
            ValidateStatusTransition(order.Status, request.Status);

          order.Status = request.Status;
            order.UpdatedAt = DateTime.UtcNow;

            // If cancelled, restore product stock
   if (request.Status == OrderStatus.Cancelled)
            {
          foreach (var item in order.OrderItems)
  {
        var product = await _productRepo.GetByIdAsync(item.ProductId);
     if (product != null)
         {
     product.Stock += item.Quantity;
          product.UpdatedAt = DateTime.UtcNow;
              }
    }
}

    await _orderRepo.UpdateAsync(order);
 await _orderRepo.SaveAsync();

         _logger.LogInformation($"Order {order.OrderCode} status updated to {request.Status}");

 return await MapToOrderResponseAsync(order);
}

        public async Task<bool> CancelOrderAsync(int orderId, int userId)
        {
      var order = await _orderRepo.GetOrderWithDetailsAsync(orderId);
            if (order == null)
return false;

            // Check if user owns the order
            if (order.UserId != userId)
    throw new UnauthorizedAccessException("B?n không có quy?n h?y ??n hàng này");

            // Only pending orders can be cancelled by customer
          if (order.Status != OrderStatus.Pending)
  throw new InvalidOperationException("Ch? có th? h?y ??n hàng ?ang ch? xác nh?n");

            // Restore product stock
      foreach (var item in order.OrderItems)
          {
        var product = await _productRepo.GetByIdAsync(item.ProductId);
         if (product != null)
          {
    product.Stock += item.Quantity;
 product.UpdatedAt = DateTime.UtcNow;
        }
        }

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

      await _orderRepo.UpdateAsync(order);
         await _orderRepo.SaveAsync();

            _logger.LogInformation($"Order {order.OrderCode} cancelled by user {userId}");

            return true;
  }

        public async Task<OrderStatisticsResponse> GetOrderStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
   var query = _orderRepo.GetAll().AsQueryable();

  if (fromDate.HasValue)
           query = query.Where(o => o.OrderDate >= fromDate.Value);

   if (toDate.HasValue)
   query = query.Where(o => o.OrderDate <= toDate.Value);

        var orders = await query.ToListAsync();

            var statistics = new OrderStatisticsResponse
     {
          TotalOrders = orders.Count,
      PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending),
          ConfirmedOrders = orders.Count(o => o.Status == OrderStatus.Confirmed),
                ShippingOrders = orders.Count(o => o.Status == OrderStatus.Shipping),
         CompletedOrders = orders.Count(o => o.Status == OrderStatus.Completed),
         CancelledOrders = orders.Count(o => o.Status == OrderStatus.Cancelled),
    TotalRevenue = orders.Where(o => o.Status == OrderStatus.Completed).Sum(o => o.TotalAmount),
  AverageOrderValue = orders.Any() ? orders.Average(o => o.TotalAmount) : 0,
                OrdersByStatus = orders.GroupBy(o => o.Status)
         .ToDictionary(g => GetStatusDisplay(g.Key), g => g.Count())
            };

    return statistics;
        }

  #region Private Helper Methods

        private async Task<string> GenerateOrderCodeAsync()
      {
          var lastOrder = await _orderRepo.GetAll()
         .OrderByDescending(o => o.Id)
     .FirstOrDefaultAsync();

       int nextNumber = 1;
   if (lastOrder != null && !string.IsNullOrEmpty(lastOrder.OrderCode))
   {
 var codeNumber = lastOrder.OrderCode.Replace("ORD", "");
       if (int.TryParse(codeNumber, out int currentNumber))
        {
     nextNumber = currentNumber + 1;
   }
      }

        return $"ORD{nextNumber:D6}"; // ORD000001, ORD000002, etc.
        }

   private async Task<OrderResponse> MapToOrderResponseAsync(Order order)
     {
      // Ensure order items are loaded
if (order.OrderItems == null || !order.OrderItems.Any())
            {
           order = await _orderRepo.GetOrderWithDetailsAsync(order.Id) ?? order;
       }

      return new OrderResponse
        {
      Id = order.Id,
      OrderCode = order.OrderCode,
         UserId = order.UserId,
   CustomerName = order.User?.FullName ?? "N/A",
       CustomerEmail = order.User?.Email ?? "N/A",
  OrderDate = order.OrderDate,
                Status = order.Status,
          StatusDisplay = GetStatusDisplay(order.Status),
         ShippingAddress = order.ShippingAddress,
                ShippingNote = order.ShippingNote,
     PaymentMethod = order.PaymentMethod,
  IsPaid = order.IsPaid,
TotalAmount = order.TotalAmount,
    Items = order.OrderItems?.Select(oi => new OrderItemResponse
     {
   Id = oi.Id,
   ProductId = oi.ProductId,
        ProductName = oi.Product?.Name ?? "N/A",
     ProductImage = oi.Product?.ImageUrl,
         Quantity = oi.Quantity,
     UnitPrice = oi.UnitPrice,
  TotalPrice = oi.TotalPrice
     }).ToList() ?? new List<OrderItemResponse>(),
  CreatedAt = order.CreatedAt,
     UpdatedAt = order.UpdatedAt
        };
   }

        private static string GetStatusDisplay(OrderStatus status)
{
      return status switch
            {
OrderStatus.Pending => "Ch? xác nh?n",
  OrderStatus.Confirmed => "?ã xác nh?n",
 OrderStatus.Shipping => "?ang giao hàng",
           OrderStatus.Completed => "Hoàn thành",
    OrderStatus.Cancelled => "?ã h?y",
         _ => "Không xác ??nh"
            };
    }

        private static void ValidateStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
     // Define valid status transitions
     var validTransitions = new Dictionary<OrderStatus, List<OrderStatus>>
            {
           { OrderStatus.Pending, new List<OrderStatus> { OrderStatus.Confirmed, OrderStatus.Cancelled } },
       { OrderStatus.Confirmed, new List<OrderStatus> { OrderStatus.Shipping, OrderStatus.Cancelled } },
          { OrderStatus.Shipping, new List<OrderStatus> { OrderStatus.Completed, OrderStatus.Cancelled } },
   { OrderStatus.Completed, new List<OrderStatus>() }, // Cannot change from completed
   { OrderStatus.Cancelled, new List<OrderStatus>() }  // Cannot change from cancelled
          };

  if (!validTransitions[currentStatus].Contains(newStatus))
        {
  throw new InvalidOperationException(
           $"Không th? chuy?n tr?ng thái t? {GetStatusDisplay(currentStatus)} sang {GetStatusDisplay(newStatus)}");
     }
        }

        #endregion
    }
}
