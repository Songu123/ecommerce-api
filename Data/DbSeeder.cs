using API.Models;
using API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    /// <summary>
    /// Database seeder for initial data
    /// </summary>
    public class DbSeeder
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DbSeeder> _logger;

        public DbSeeder(AppDbContext context, ILogger<DbSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Seed all data
        /// </summary>
        public async Task SeedAsync()
        {
            try
            {
                // Ensure database is created
                await _context.Database.MigrateAsync();

                // Seed data in order
                await SeedCategoriesAsync();
                await SeedUsersAsync();
                await SeedProductsAsync();
                await SeedOrdersAsync();

                _logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }

        /// <summary>
        /// Seed categories
        /// </summary>
        private async Task SeedCategoriesAsync()
        {
            if (await _context.Categories.AnyAsync())
            {
                _logger.LogInformation("Categories already exist, skipping seed");
                return;
            }

            var categories = new List<Category>
            {
      new Category
      {
            Name = "Laptops",
       Description = "High-performance laptops for work and gaming",
     CreatedAt = DateTime.UtcNow
    },
           new Category
 {
            Name = "Smartphones",
      Description = "Latest smartphones with cutting-edge technology",
         CreatedAt = DateTime.UtcNow
    },
    new Category
    {
     Name = "Cameras",
          Description = "Professional cameras and photography equipment",
          CreatedAt = DateTime.UtcNow
                },
   new Category
     {
            Name = "Accessories",
  Description = "Electronic accessories and peripherals",
      CreatedAt = DateTime.UtcNow
   },
        new Category
 {
          Name = "Tablets",
         Description = "Tablets for entertainment and productivity",
         CreatedAt = DateTime.UtcNow
  },
 new Category
   {
        Name = "Headphones",
      Description = "Premium audio devices and headphones",
  CreatedAt = DateTime.UtcNow
                }
  };

            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {categories.Count} categories");
        }

        /// <summary>
        /// Seed users
        /// </summary>
        private async Task SeedUsersAsync()
        {
            if (await _context.Users.AnyAsync())
            {
                _logger.LogInformation("Users already exist, skipping seed");
                return;
            }

            var users = new List<User>
          {
                new User
             {
          FullName = "Admin User",
     Email = "admin@electro.com",
    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
     Role = "Admin",
         IsActive = true,
       CreatedAt = DateTime.UtcNow
         },
 new User
    {
     FullName = "John Doe",
           Email = "john.doe@example.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
         Role = "Customer",
         IsActive = true,
CreatedAt = DateTime.UtcNow
       },
  new User
          {
       FullName = "Jane Smith",
          Email = "jane.smith@example.com",
      PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
   Role = "Customer",
 IsActive = true,
    CreatedAt = DateTime.UtcNow
     },
        new User
         {
         FullName = "Bob Johnson",
           Email = "bob.johnson@example.com",
         PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
   Role = "Customer",
           IsActive = true,
         CreatedAt = DateTime.UtcNow
  }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {users.Count} users");
        }

        /// <summary>
        /// Seed products
        /// </summary>
        private async Task SeedProductsAsync()
        {
            if (await _context.Products.AnyAsync())
            {
                _logger.LogInformation("Products already exist, skipping seed");
                return;
            }

            // Get categories
            var laptopCategory = await _context.Categories.FirstAsync(c => c.Name == "Laptops");
            var smartphoneCategory = await _context.Categories.FirstAsync(c => c.Name == "Smartphones");
            var cameraCategory = await _context.Categories.FirstAsync(c => c.Name == "Cameras");
            var accessoryCategory = await _context.Categories.FirstAsync(c => c.Name == "Accessories");
            var tabletCategory = await _context.Categories.FirstAsync(c => c.Name == "Tablets");
            var headphoneCategory = await _context.Categories.FirstAsync(c => c.Name == "Headphones");

            var products = new List<Product>
            {
 // Laptops
           new Product
       {
       Name = "Dell XPS 13",
    Price = 1299.99M,
                Description = "Powerful ultrabook with 11th Gen Intel Core processor",
        ImageUrl = "/img/product01.png",
     Stock = 25,
     CategoryId = laptopCategory.Id,
  CreatedAt = DateTime.UtcNow
     },
    new Product
   {
            Name = "MacBook Pro 16",
  Price = 2499.99M,
    Description = "Professional laptop with M1 Pro chip and stunning display",
        ImageUrl = "/img/product02.png",
          Stock = 15,
         CategoryId = laptopCategory.Id,
        CreatedAt = DateTime.UtcNow
              },
                new Product
     {
     Name = "HP Pavilion Gaming",
           Price = 899.99M,
    Description = "Gaming laptop with NVIDIA GTX graphics",
         ImageUrl = "/img/product03.png",
     Stock = 30,
     CategoryId = laptopCategory.Id,
       CreatedAt = DateTime.UtcNow
      },
        new Product
        {
              Name = "Lenovo ThinkPad X1",
    Price = 1599.99M,
        Description = "Business laptop with excellent keyboard and security features",
          ImageUrl = "/img/product04.png",
         Stock = 20,
   CategoryId = laptopCategory.Id,
            CreatedAt = DateTime.UtcNow
       },
     
  // Smartphones
new Product
{
        Name = "iPhone 14 Pro",
        Price = 1099.99M,
          Description = "Latest iPhone with Dynamic Island and A16 Bionic chip",
      ImageUrl = "/img/product05.png",
  Stock = 50,
      CategoryId = smartphoneCategory.Id,
       CreatedAt = DateTime.UtcNow
            },
              new Product
 {
                    Name = "Samsung Galaxy S23",
    Price = 899.99M,
     Description = "Flagship Android phone with excellent camera system",
   ImageUrl = "/img/product06.png",
     Stock = 45,
      CategoryId = smartphoneCategory.Id,
        CreatedAt = DateTime.UtcNow
       },
     new Product
 {
            Name = "Google Pixel 7",
            Price = 599.99M,
 Description = "Pure Android experience with amazing computational photography",
         ImageUrl = "/img/product07.png",
           Stock = 35,
       CategoryId = smartphoneCategory.Id,
        CreatedAt = DateTime.UtcNow
      },
      new Product
          {
          Name = "OnePlus 11",
         Price = 699.99M,
     Description = "Fast charging flagship with smooth performance",
            ImageUrl = "/img/product08.png",
        Stock = 40,
    CategoryId = smartphoneCategory.Id,
         CreatedAt = DateTime.UtcNow
       },

         // Cameras
           new Product
    {
       Name = "Canon EOS R6",
    Price = 2499.99M,
 Description = "Full-frame mirrorless camera for professionals",
     ImageUrl = "/img/product09.png",
   Stock = 10,
          CategoryId = cameraCategory.Id,
                CreatedAt = DateTime.UtcNow
 },
             new Product
   {
          Name = "Sony A7 IV",
  Price = 2799.99M,
             Description = "Versatile hybrid camera for photo and video",
          ImageUrl = "/img/product01.png",
 Stock = 12,
         CategoryId = cameraCategory.Id,
          CreatedAt = DateTime.UtcNow
       },
 new Product
       {
      Name = "Nikon Z6 II",
Price = 1999.99M,
      Description = "Dual processor mirrorless camera with great low-light performance",
             ImageUrl = "/img/product02.png",
     Stock = 8,
        CategoryId = cameraCategory.Id,
    CreatedAt = DateTime.UtcNow
     },
             new Product
     {
      Name = "Fujifilm X-T4",
          Price = 1699.99M,
      Description = "APS-C camera with in-body stabilization and film simulations",
     ImageUrl = "/img/product03.png",
            Stock = 15,
     CategoryId = cameraCategory.Id,
    CreatedAt = DateTime.UtcNow
         },

         // Accessories
          new Product
           {
                    Name = "Logitech MX Master 3",
           Price = 99.99M,
                 Description = "Advanced wireless mouse for productivity",
   ImageUrl = "/img/product04.png",
               Stock = 100,
   CategoryId = accessoryCategory.Id,
  CreatedAt = DateTime.UtcNow
    },
        new Product
      {
            Name = "Apple Magic Keyboard",
 Price = 149.99M,
 Description = "Wireless keyboard with numeric keypad",
        ImageUrl = "/img/product05.png",
         Stock = 75,
   CategoryId = accessoryCategory.Id,
     CreatedAt = DateTime.UtcNow
         },
         new Product
   {
                    Name = "USB-C Hub 7-in-1",
     Price = 49.99M,
            Description = "Multi-port adapter with HDMI, USB, and SD card reader",
             ImageUrl = "/img/product06.png",
            Stock = 150,
             CategoryId = accessoryCategory.Id,
            CreatedAt = DateTime.UtcNow
       },
new Product
          {
                    Name = "Anker PowerBank 20000mAh",
   Price = 59.99M,
         Description = "High-capacity portable charger with fast charging",
                    ImageUrl = "/img/product07.png",
           Stock = 120,
         CategoryId = accessoryCategory.Id,
       CreatedAt = DateTime.UtcNow
      },

   // Tablets
        new Product
              {
            Name = "iPad Pro 12.9",
         Price = 1099.99M,
 Description = "Powerful tablet with M2 chip and ProMotion display",
        ImageUrl = "/img/product08.png",
               Stock = 30,
      CategoryId = tabletCategory.Id,
            CreatedAt = DateTime.UtcNow
     },
  new Product
                {
         Name = "Samsung Galaxy Tab S8",
           Price = 699.99M,
   Description = "Premium Android tablet with S Pen included",
        ImageUrl = "/img/product09.png",
  Stock = 25,
          CategoryId = tabletCategory.Id,
      CreatedAt = DateTime.UtcNow
                },
 new Product
  {
          Name = "Microsoft Surface Pro 9",
        Price = 999.99M,
              Description = "2-in-1 tablet and laptop with Windows 11",
  ImageUrl = "/img/product01.png",
      Stock = 20,
 CategoryId = tabletCategory.Id,
      CreatedAt = DateTime.UtcNow
                },

   // Headphones
        new Product
        {
        Name = "Sony WH-1000XM5",
   Price = 399.99M,
     Description = "Industry-leading noise cancelling headphones",
      ImageUrl = "/img/product02.png",
              Stock = 60,
                CategoryId = headphoneCategory.Id,
       CreatedAt = DateTime.UtcNow
             },
     new Product
      {
        Name = "Apple AirPods Pro 2",
            Price = 249.99M,
   Description = "Wireless earbuds with active noise cancellation",
        ImageUrl = "/img/product03.png",
       Stock = 80,
   CategoryId = headphoneCategory.Id,
    CreatedAt = DateTime.UtcNow
          },
  new Product
      {
            Name = "Bose QuietComfort 45",
     Price = 329.99M,
       Description = "Premium comfort with world-class noise cancellation",
               ImageUrl = "/img/product04.png",
        Stock = 50,
CategoryId = headphoneCategory.Id,
           CreatedAt = DateTime.UtcNow
            },
              new Product
       {
        Name = "JBL Flip 6",
    Price = 129.99M,
      Description = "Portable Bluetooth speaker with powerful sound",
     ImageUrl = "/img/product05.png",
     Stock = 90,
  CategoryId = headphoneCategory.Id,
        CreatedAt = DateTime.UtcNow
        }
     };

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {products.Count} products");
        }

        /// <summary>
        /// Seed orders
        /// </summary>
        private async Task SeedOrdersAsync()
        {
   if (await _context.Orders.AnyAsync())
 {
                _logger.LogInformation("Orders already exist, skipping seed");
   return;
    }

      var users = await _context.Users.Where(u => u.Role == "Customer").ToListAsync();
            var products = await _context.Products.ToListAsync();

    if (!users.Any() || !products.Any())
  {
             _logger.LogWarning("Cannot seed orders without users and products");
    return;
            }

  var random = new Random();
  var orders = new List<Order>();

   // ??a ch? m?u ? Vi?t Nam
   var addresses = new[]
      {
          "123 Nguy?n Hu?, Qu?n 1, TP. H? Chí Minh",
                "456 Lê L?i, Qu?n 1, TP. H? Chí Minh",
    "789 Tr?n H?ng ??o, Qu?n 5, TP. H? Chí Minh",
         "321 Võ V?n T?n, Qu?n 3, TP. H? Chí Minh",
      "654 Nguy?n Th? Minh Khai, Qu?n 3, TP. H? Chí Minh",
            "147 Hai Bà Tr?ng, Qu?n 1, TP. H? Chí Minh",
       "258 ?i?n Biên Ph?, Qu?n Bình Th?nh, TP. H? Chí Minh",
         "369 Cách M?ng Tháng 8, Qu?n 10, TP. H? Chí Minh",
 "741 Hoàng V?n Th?, Qu?n Tân Bình, TP. H? Chí Minh",
         "852 Phan ??ng L?u, Qu?n Phú Nhu?n, TP. H? Chí Minh"
     };

          var shippingNotes = new[]
      {
      "Giao hàng gi? hành chính",
       "G?i tr??c khi giao",
    "Giao t?n tay, không giao cho ng??i khác",
             "?? tr??c c?a n?u không có ng??i",
 "Liên h? qua s? ?i?n tho?i tr??c khi giao",
       null // Không có ghi chú
         };

            var paymentMethods = new[]
       {
  "COD - Thanh toán khi nh?n hàng",
     "Chuy?n kho?n ngân hàng",
                "Ví ?i?n t? MoMo",
 "Ví ?i?n t? ZaloPay",
          "Th? tín d?ng/Ghi n?"
    };

            // T?o 20 ??n hàng m?u v?i d? li?u th?c t?
     for (int i = 1; i <= 20; i++)
    {
       var user = users[random.Next(users.Count)];
       var orderDate = DateTime.UtcNow.AddDays(-random.Next(0, 45));
          
 // Random status d?a trên ngày ??t - logic th?c t? h?n
   OrderStatus status;
       bool isPaid;

         if (orderDate >= DateTime.UtcNow.AddDays(-2))
      {
         // ??n hàng m?i: 70% Pending, 30% Confirmed
    status = random.Next(10) < 7 ? OrderStatus.Pending : OrderStatus.Confirmed;
        isPaid = false;
       }
            else if (orderDate >= DateTime.UtcNow.AddDays(-7))
       {
            // ??n hàng 2-7 ngày: 40% Confirmed, 40% Shipping, 20% Cancelled
            var statusRoll = random.Next(10);
              status = statusRoll switch
  {
       < 4 => OrderStatus.Confirmed,
             < 8 => OrderStatus.Shipping,
     _ => OrderStatus.Cancelled
        };
       isPaid = status == OrderStatus.Shipping || random.Next(2) == 0;
           }
     else if (orderDate >= DateTime.UtcNow.AddDays(-30))
     {
            // ??n hàng 7-30 ngày: 60% Completed, 30% Shipping, 10% Cancelled
            var statusRoll = random.Next(10);
          status = statusRoll switch
      {
 < 6 => OrderStatus.Completed,
               < 9 => OrderStatus.Shipping,
           _ => OrderStatus.Cancelled
        };
         isPaid = status == OrderStatus.Completed || status == OrderStatus.Shipping;
    }
        else
         {
                  // ??n hàng c?: 80% Completed, 20% Cancelled
    status = random.Next(10) < 8 ? OrderStatus.Completed : OrderStatus.Cancelled;
    isPaid = status == OrderStatus.Completed;
  }

         var order = new Order
         {
                OrderCode = $"ORD{(100000 + i):D6}",
                 UserId = user.Id,
         OrderDate = orderDate,
        Status = status,
  ShippingAddress = addresses[random.Next(addresses.Length)],
   ShippingNote = shippingNotes[random.Next(shippingNotes.Length)],
     PaymentMethod = paymentMethods[random.Next(paymentMethods.Length)],
    IsPaid = isPaid,
        TotalAmount = 0, // Will calculate after adding items
          CreatedAt = orderDate,
     UpdatedAt = status != OrderStatus.Pending ? orderDate.AddHours(random.Next(1, 72)) : null
  };

// Thêm 1-4 s?n ph?m vào m?i order
       var itemCount = random.Next(1, 5);
           var orderItems = new List<OrderItem>();
 var selectedProductIds = new HashSet<int>();
   decimal totalAmount = 0;

    for (int j = 0; j < itemCount; j++)
        {
      // ??m b?o không có s?n ph?m trùng l?p trong cùng 1 order
          Product product;
          do
  {
            product = products[random.Next(products.Count)];
    }
        while (selectedProductIds.Contains(product.Id));
            
       selectedProductIds.Add(product.Id);
       var quantity = random.Next(1, 3);

   var orderItem = new OrderItem
         {
   Order = order,
      ProductId = product.Id,
           Quantity = quantity,
          UnitPrice = product.Price,
       CreatedAt = orderDate
     };

  orderItems.Add(orderItem);
    totalAmount += orderItem.TotalPrice;
             }

  order.OrderItems = orderItems;
  order.TotalAmount = totalAmount;
         orders.Add(order);
          }

await _context.Orders.AddRangeAsync(orders);
      await _context.SaveChangesAsync();
     _logger.LogInformation($"Seeded {orders.Count} orders with {orders.Sum(o => o.OrderItems.Count)} order items. Total revenue: {orders.Where(o => o.Status == OrderStatus.Completed).Sum(o => o.TotalAmount):C}");
    }
    }
}
