namespace CompanyEmployees.Api.Extenstions;
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
