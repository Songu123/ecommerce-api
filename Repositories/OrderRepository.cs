using API.Data;
using API.Models;
using API.Models.Enums;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    /// <summary>
    /// Order repository implementation
  /// </summary>
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly AppDbContext _appContext;

        public OrderRepository(AppDbContext context) : base(context)
     {
  _appContext = context;
   }

        public async Task<Order?> GetByOrderCodeAsync(string orderCode)
    {
         return await _appContext.Orders
                .Include(o => o.User)
          .Include(o => o.OrderItems)
           .ThenInclude(oi => oi.Product)
    .FirstOrDefaultAsync(o => o.OrderCode == orderCode);
    }

        public async Task<List<Order>> GetByUserIdAsync(int userId)
   {
         return await _appContext.Orders
         .Where(o => o.UserId == userId)
    .Include(o => o.OrderItems)
          .ThenInclude(oi => oi.Product)
             .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
        }

        public async Task<List<Order>> GetByStatusAsync(OrderStatus status)
        {
    return await _appContext.Orders
.Where(o => o.Status == status)
          .Include(o => o.User)
        .Include(o => o.OrderItems)
             .ThenInclude(oi => oi.Product)
    .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
   }

        public async Task<Order?> GetOrderWithDetailsAsync(int id)
   {
       return await _appContext.Orders
    .Include(o => o.User)
 .Include(o => o.OrderItems)
         .ThenInclude(oi => oi.Product)
           .ThenInclude(p => p.Category)
.FirstOrDefaultAsync(o => o.Id == id);
        }

      public async Task<List<Order>> GetUserOrdersWithDetailsAsync(int userId)
        {
       return await _appContext.Orders
  .Where(o => o.UserId == userId)
       .Include(o => o.OrderItems)
 .ThenInclude(oi => oi.Product)
  .OrderByDescending(o => o.OrderDate)
     .ToListAsync();
  }

public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
     {
 var order = await _appContext.Orders.FindAsync(orderId);
        if (order != null)
     {
      order.Status = status;
       order.UpdatedAt = DateTime.UtcNow;
        await _appContext.SaveChangesAsync();
        }
        }

     public async Task<(List<Order> Orders, int TotalCount)> GetOrdersPagedAsync(
         int page,
   int pageSize,
            OrderStatus? status = null,
      int? userId = null,
DateTime? fromDate = null,
     DateTime? toDate = null)
   {
      var query = _appContext.Orders
      .Include(o => o.User)
    .Include(o => o.OrderItems)
       .ThenInclude(oi => oi.Product)
       .AsQueryable();

     // Apply filters
  if (status.HasValue)
         query = query.Where(o => o.Status == status.Value);

       if (userId.HasValue)
        query = query.Where(o => o.UserId == userId.Value);

  if (fromDate.HasValue)
    query = query.Where(o => o.OrderDate >= fromDate.Value);

         if (toDate.HasValue)
             query = query.Where(o => o.OrderDate <= toDate.Value);

     var totalCount = await query.CountAsync();

            var orders = await query
         .OrderByDescending(o => o.OrderDate)
         .Skip((page - 1) * pageSize)
               .Take(pageSize)
           .ToListAsync();

  return (orders, totalCount);
        }
    }
}
