using CompanyEmployees.Api.Data;
using CompanyEmployees.Api.Extenstions;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

namespace CompanyEmployees.Api;

public class Program
{
    public static void Main(string[] args)
    {
        // Early init of NLog to allow startup and exception logging, before host is built
        var logger = LogManager.Setup().LoadConfigurationFromFile(optional: false).GetCurrentClassLogger();
        logger.Debug("init main");

        try
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            // Add services to the container.

            builder.Services.AddDbContext<CompanyContext>(opts =>
                opts.UseSqlite(builder.Configuration.GetConnectionString("sqliteConnection")));

            builder.Services.AddControllers();

            builder.Services.ConfigureCors();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseHttpsRedirection();


            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }
        catch (Exception ex)
        {
            logger.Fatal("Stopped program because of {exception}", ex);
            throw;
        }
        finally
        {
            LogManager.Shutdown();
        }

    }
}
