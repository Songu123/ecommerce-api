using API.Data;
using API.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    /// <summary>
    /// Seed controller for database seeding operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class SeedController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DbSeeder> _logger;
        private readonly ILogger<SeedController> _controllerLogger;

        public SeedController(
            AppDbContext context,
            ILogger<DbSeeder> logger,
            ILogger<SeedController> controllerLogger)
        {
            _context = context;
            _logger = logger;
            _controllerLogger = controllerLogger;
        }

        /// <summary>
        /// Manually trigger database seeding
        /// </summary>
        /// <returns>Success message</returns>
        /// <response code="200">Database seeded successfully</response>
        /// <response code="401">Unauthorized - Admin access required</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("seed")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SeedDatabase()
        {
            try
            {
                var seeder = new DbSeeder(_context, _logger);
                await seeder.SeedAsync();

                return Ok(ApiResponse<string>.SuccessResponse(
                    "Database seeded successfully",
                    "All seed data has been inserted into the database"
                ));
            }
            catch (Exception ex)
            {
                _controllerLogger.LogError(ex, "Error occurred while seeding database");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Failed to seed database",
                    new List<string> { ex.Message }
                ));
            }
        }

        /// <summary>
        /// Get seeding status and information
        /// </summary>
        /// <returns>Seeding information</returns>
        /// <response code="200">Returns seeding information</response>
        /// <response code="401">Unauthorized - Admin access required</response>
        [HttpGet("status")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSeedStatus()
        {
            var categoriesCount = await _context.Categories.CountAsync();
            var productsCount = await _context.Products.CountAsync();
            var usersCount = await _context.Users.CountAsync();

            var status = new
            {
                IsSeeded = categoriesCount > 0 && productsCount > 0 && usersCount > 0,
                Statistics = new
                {
                    Categories = categoriesCount,
                    Products = productsCount,
                    Users = usersCount
                },
                ExpectedCounts = new
                {
                    Categories = 6,
                    Products = 27,
                    Users = 4
                },
                Message = categoriesCount > 0 && productsCount > 0 && usersCount > 0
                    ? "Database has been seeded"
                    : "Database has not been seeded yet"
            };

            return Ok(new
            {
                Success = true,
                Message = "Seed status retrieved successfully",
                Data = status
            });
        }
    }
}
