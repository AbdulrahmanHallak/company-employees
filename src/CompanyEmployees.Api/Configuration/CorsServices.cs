namespace CompanyEmployees.Api.Configuration;
public static class CorsServices
{
    public static IServiceCollection ConfigureCors(this IServiceCollection services)
                => services.AddCors(opts =>
                {
                    opts.AddPolicy("CorsPolicy", builder =>
                        builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
                });

}
