using API.DTOs.Request;
using API.DTOs.Response;
using API.Helpers;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

return new AuthResponse
    {
   UserId = user.Id,
        FullName = user.FullName,
       Email = user.Email,
     Role = user.Role,
      Token = token,
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

return new AuthResponse
     {
     UserId = user.Id,
     FullName = user.FullName,
Email = user.Email,
        Role = user.Role,
    Token = token,
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
    }
}
