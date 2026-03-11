using API.DTOs.Request;
using API.DTOs.Response;
using API.Helpers;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace API.Services
{
    /// <summary>
    /// Authentication service implementation
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
           IUserRepository userRepo,
                    IOptions<JwtSettings> jwtSettings,
             ILogger<AuthService> logger)
        {
            _userRepo = userRepo;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Email ho?c m?t kh?u không ?úng");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Tài kho?n ?ã b? khóa");
            }

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            // Save refresh token to database
            var refreshTokenExpiryDate = DateTime.UtcNow.AddDays(7); // 7 days
            await _userRepo.SaveRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiryDate);

            return new AuthResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Validate email uniqueness
            if (await _userRepo.IsEmailExistsAsync(request.Email))
            {
                throw new InvalidOperationException("Email ?ã ???c s? d?ng");
            }

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "Customer",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepo.AddAsync(user);
            await _userRepo.SaveAsync();

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            // Save refresh token to database
            var refreshTokenExpiryDate = DateTime.UtcNow.AddDays(7); // 7 days
            await _userRepo.SaveRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiryDate);

            return new AuthResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
            };
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(TokenRequest tokenRequest)
        {
            var principal = GetPrincipalFromExpiredToken(tokenRequest.Token);

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new SecurityTokenException("Invalid token claims");
            }

            var userId = int.Parse(userIdClaim);

            var storedRefreshToken = await _userRepo.GetRefreshTokenAsync(userId);

            if (storedRefreshToken == null)
            {
                throw new SecurityTokenException("Refresh token not found");
            }

            if (storedRefreshToken.Token != tokenRequest.RefreshToken)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            if (storedRefreshToken.ExpiryDate <= DateTime.UtcNow)
            {
                throw new SecurityTokenException("Refresh token has expired");
            }

            if (storedRefreshToken.IsRevoked)
            {
                throw new SecurityTokenException("Refresh token has been revoked");
            }

            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
            {
                throw new SecurityTokenException("User not found");
            }

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // Save new refresh token and revoke old one
            var refreshTokenExpiryDate = DateTime.UtcNow.AddDays(7); // 7 days
            await _userRepo.SaveRefreshTokenAsync(userId, newRefreshToken, refreshTokenExpiryDate);

            return new AuthResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
            };
        }

        /// <summary>
        /// Generate JWT token for user
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var claims = new List<Claim>
            {
      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
              new Claim(ClaimTypes.Name, user.FullName),
     new Claim(ClaimTypes.Email, user.Email),
  new Claim(ClaimTypes.Role, user.Role),
     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
   };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                  SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = false // We want to get claims from expired token
            }, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
    }
}
