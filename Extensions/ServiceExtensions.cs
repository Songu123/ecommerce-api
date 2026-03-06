using API.Filters;
using API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace API.Extensions
{
    /// <summary>
    /// Extension methods ?? ??ng ký services
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// C?u hình CORS
        /// </summary>
        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
        {
            services.AddCors(options =>
                 {
                     options.AddPolicy("AllowAll", builder =>
          {
              builder.AllowAnyOrigin()
.AllowAnyMethod()
.AllowAnyHeader();
          });

                     options.AddPolicy("AllowSpecific", builder =>
            {
                builder.WithOrigins("http://localhost:3000", "https://yourdomain.com")
 .AllowAnyMethod()
  .AllowAnyHeader()
.AllowCredentials();
            });
                 });

            return services;
        }

        /// <summary>
        /// C?u hình Controllers v?i Filters
        /// </summary>
        public static IServiceCollection AddControllersConfiguration(this IServiceCollection services)
        {
            services.AddControllers(options =>
       {
           // Thêm global filters
           options.Filters.Add<ValidateModelStateFilter>();
           options.Filters.Add<GlobalExceptionFilter>();
       })
           .AddJsonOptions(options =>
            {
                // C?u hình JSON serialization
                options.JsonSerializerOptions.ReferenceHandler =
               System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.DefaultIgnoreCondition =
                System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });

            return services;
        }

        /// <summary>
        /// C?u hình Swagger v?i JWT Bearer
        /// </summary>
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new()
                    {
                        Title = "E-commerce API",
                        Version = "v1"
                    });

                    // Thêm JWT Bearer authentication vào Swagger
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
            new OpenApiSecurityScheme
     {
         Reference = new OpenApiReference
                {
  Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
                  }
      },
         Array.Empty<string>()
         }
             });

                    // Include XML comments (n?u có)
                    // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    // options.IncludeXmlComments(xmlPath);
                });

            return services;
        }

        /// <summary>
        /// C?u hình JWT Authentication
        /// </summary>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            services.Configure<JwtSettings>(jwtSettings);

            var secretKey = jwtSettings.Get<JwtSettings>()?.SecretKey ?? throw new InvalidOperationException("JWT SecretKey not configured");

            services.AddAuthentication(options =>
       {
           options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
           options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
       })
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                       ValidateIssuer = true,
                       ValidIssuer = jwtSettings.Get<JwtSettings>()?.Issuer,
                       ValidateAudience = true,
                       ValidAudience = jwtSettings.Get<JwtSettings>()?.Audience,
                       ValidateLifetime = true,
                       ClockSkew = TimeSpan.Zero
                   };
               });

            return services;
        }
    }
}
