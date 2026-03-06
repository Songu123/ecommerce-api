using API.DTOs.Response;
using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
/// <summary>
    /// Base Controller v?i các helper methods chung
    /// </summary>
    [ApiController]
    [Route(ApiConstants.ApiPrefix + "/[controller]")]
    [Produces("application/json")]
    public abstract class BaseApiController : ControllerBase
    {
     /// <summary>
    /// Tr? v? success response
        /// </summary>
  protected IActionResult SuccessResponse<T>(T data, string message = "Success")
   {
  return Ok(ApiResponse<T>.SuccessResponse(data, message));
        }

  /// <summary>
        /// Tr? v? error response
        /// </summary>
   protected IActionResult ErrorResponse(string message, List<string>? errors = null)
 {
   return BadRequest(ApiResponse<object>.ErrorResponse(message, errors));
        }

        /// <summary>
        /// Tr? v? not found response
        /// </summary>
 protected IActionResult NotFoundResponse(string message = "Resource not found")
   {
   return NotFound(ApiResponse<object>.ErrorResponse(message));
}

        /// <summary>
    /// Tr? v? paginated response
 /// </summary>
 protected IActionResult PaginatedResponse<T>(PaginatedResponse<T> data)
        {
     return Ok(data);
 }
    }
}
