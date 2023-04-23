using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopAPI.BL.Managers;
using ShopAPI.BL.Managers.User;
using ShopAPI.BL.Mapper;
using ShopAPI.DAL.Data.Context;
using ShopAPI.DAL.Data.Models;
using ShopAPI.DAL.Repository;
using System.Security.Claims;
using System.Text;

namespace ShopAPI.Services;

public static class ServiceExtension
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(option =>
        {
            option.AddPolicy("allowAll", policy =>
            {
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowAnyOrigin();
            });
        });
    }

    public static void ConfigureDB(this IServiceCollection services, string ConnectionString)
    {
        services.AddDbContext<ShopDbContext>(options =>
        {
            options.UseSqlServer(ConnectionString);
        });
    }

    public static void ConfigureRepository(this IServiceCollection services)
    {
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
    }
    public static void ConfigureMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MapperProfile));
    }
    public static void ConfigureManagers(this IServiceCollection services)
    {
        services.AddScoped<IProductManager, ProductManager>();
        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<IOrderManager, OrderManager>();
    }
    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole<string>>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<ShopDbContext>();
    }
    public static void ConfigureAuthentication(this IServiceCollection services, string? key)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "JWT";
            options.DefaultChallengeScheme = "JWT";
        })
                .AddJwtBearer("JWT", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key ?? "JWTSecurityKey"))
                    };
                });
    }

    public static void ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
            {
                policy.RequireClaim(ClaimTypes.Role, "Admin");
            });
            options.AddPolicy("UsersOnly", policy =>
            {
                policy.RequireClaim(ClaimTypes.Role, "User");
            });
        });
    }
}
