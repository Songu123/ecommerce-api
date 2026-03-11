using API.DTOs.Request;
using API.Services.Interfaces;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    /// <summary>
    /// Authentication API Controller
    /// </summary>
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
          IAuthService authService,
           ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// User login
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>Authentication token and user info</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
                if (!ModelState.IsValid)
                return ErrorResponse("D? li?u không h?p l?", ModelState.Values
                     .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList());

            try
            {
                var result = await _authService.LoginAsync(request);
                return SuccessResponse(result, "??ng nh?p thành công");
            }
            catch (UnauthorizedAccessException ex)
            {
                return ErrorResponse(ex.Message);
            }
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        /// <param name="request">Token request containing access token and refresh token</param>
        /// <returns>New access token and refresh token</returns>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest request)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("D? li?u không h?p l?", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList());

            try
            {
                var response = await _authService.RefreshTokenAsync(request);
                return SuccessResponse(response, "Token ?ã ???c làm m?i");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return ErrorResponse(ex.Message);
            }
        }

            /// <summary>
            /// User registration
            /// </summary>
            /// <param name="request">Registration data</param>
            /// <returns>Authentication token and user info</returns>
            [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("D? li?u không h?p l?", ModelState.Values
                    .SelectMany(v => v.Errors)
              .Select(e => e.ErrorMessage)
                .ToList());

            try
            {
                var result = await _authService.RegisterAsync(request);
                return SuccessResponse(result, "??ng ký thành công");
            }
            catch (InvalidOperationException ex)
            {
                return ErrorResponse(ex.Message);
            }
        }

        /// <summary>
        /// Validate JWT token
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>Validation result</returns>
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateToken([FromBody] string token)
        {
            var isValid = await _authService.ValidateTokenAsync(token);

            if (!isValid)
                return ErrorResponse("Token không h?p l? ho?c ?ã h?t h?n");

            return SuccessResponse(true, "Token h?p l?");
        }

        /// <summary>
        /// Get current user info (requires authentication)
        /// </summary>
        /// <returns>Current user information</returns>
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            return SuccessResponse(new
            {
                UserId = userId,
                FullName = userName,
                Email = userEmail,
                Role = userRole
            });
        }
    }
}
