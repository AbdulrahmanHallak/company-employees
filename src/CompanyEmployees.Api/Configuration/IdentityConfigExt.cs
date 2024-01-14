using CompanyEmployees.Api.Data;
using CompanyEmployees.Api.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace CompanyEmployees.Api.Configuration;
public static class IdentityConfigExt
{
    public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>(opts =>
        {
            opts.Password.RequireDigit = true;
            opts.Password.RequiredLength = 10;
            opts.Password.RequireLowercase = false;
            opts.Password.RequireUppercase = false;
            opts.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }
}
