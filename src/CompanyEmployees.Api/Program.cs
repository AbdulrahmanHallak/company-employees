using CompanyEmployees.Api.Extenstions;
using Microsoft.AspNetCore.HttpOverrides;

namespace CompanyEmployees.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        builder.Services.ConfigureCors();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
            app.UseHsts();

        app.UseHttpsRedirection();

        app.UseForwardedHeaders(new ForwardedHeadersOptions()
        {
            ForwardedHeaders = ForwardedHeaders.All
        });

        app.UseCors("CorsPolicy");

        app.UseAuthorization();

        app.MapControllers();

        app.Run();

    }
}
