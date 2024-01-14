using System.Reflection;

namespace CompanyEmployees.Api.Configuration;

static class RegisterServicesExt
{
    public static IServiceCollection AddServiceSuffix(this IServiceCollection services)
    {
        Assembly assembly = typeof(RegisterServicesExt).Assembly;
        IEnumerable<Type> serviceTypes =
            from type in assembly.GetTypes()
            where !type.IsAbstract
            where type.Name.EndsWith("Service")
            select type;

        foreach (Type type in serviceTypes)
            services.AddScoped(type.GetInterfaces().Single(), type);

        return services;
    }
}