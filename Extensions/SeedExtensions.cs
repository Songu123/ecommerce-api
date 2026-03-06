using API.Data;

namespace API.Extensions
{
    /// <summary>
    /// Extension methods for seeding database
    /// </summary>
    public static class SeedExtensions
    {
 /// <summary>
        /// Seed database with initial data
        /// </summary>
 public static async Task SeedDatabaseAsync(this WebApplication app)
    {
   using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

   try
{
         var context = services.GetRequiredService<AppDbContext>();
      var logger = services.GetRequiredService<ILogger<DbSeeder>>();
      
         var seeder = new DbSeeder(context, logger);
         await seeder.SeedAsync();
          }
            catch (Exception ex)
            {
             var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while seeding the database");
   }
  }
    }
}
