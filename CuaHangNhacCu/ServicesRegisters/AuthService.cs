using CuaHangNhacCu.Data;
using CuaHangNhacCu.Models;
using Microsoft.AspNetCore.Identity;

namespace CuaHangNhacCu.ServicesRegisters;

public static class AuthService
{

    public static IServiceCollection AddAuthServices(
        this IServiceCollection services
    ) 
    {
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
