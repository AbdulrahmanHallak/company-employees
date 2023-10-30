using CompanyEmployees.Api.Data;
using CompanyEmployees.Api.Configuration;
using CompanyEmployees.Api.Interfaces;
using CompanyEmployees.Api.Services;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using FluentValidation;
using CompanyEmployees.Api.Validators;
using CompanyEmployees.Api.Extensions;
using System.Threading.RateLimiting;

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

            builder.Services.AddDbContext<AppDbContext>(opts =>
                opts.UseSqlite(builder.Configuration.GetConnectionString("sqliteConnection")
                ?? throw new ArgumentNullException("The connection string cannot be null")));

            builder.Services.AddControllers(opts =>
            {
                opts.RespectBrowserAcceptHeader = true;
                opts.ReturnHttpNotAcceptable = true;
                opts.OutputFormatters.Add(new CsvOutputFormatter());
                opts.InputFormatters.Insert(0, JsonPatchInputFormatter.GetJsonPatchInputFormatter());
            })
            .ConfigureApiBehaviorOptions(opts =>
            {
                opts.SuppressModelStateInvalidFilter = true;

            }).AddXmlDataContractSerializerFormatters();

            builder.Services.AddScoped<ICompanyService, CompanyService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();

            builder.Services.AddValidatorsFromAssemblyContaining<CompanyForCreateValidator>();

            builder.Services.ConfigureVersioning();

            builder.Services.ConfigureRateLimiting();

            builder.Services.ConfigureCors();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseHttpsRedirection();

            app.UseRateLimiter();

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
