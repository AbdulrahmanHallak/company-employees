using CompanyEmployees.Api.Data;
using CompanyEmployees.Api.Configuration;
using CompanyEmployees.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using FluentValidation;
using CompanyEmployees.Api.Validators;
using CompanyEmployees.Api.ConfigModels;
using CompanyEmployees.Api.Data.Services;

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
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

            builder.Services.AddValidatorsFromAssemblyContaining<CompanyForCreateValidator>();

            builder.Services.ConfigureVersioning();

            builder.Services.ConfigureRateLimiting();

            builder.Services.ConfigureIdentity();
            builder.Services.ConfigureJWT(builder.Configuration);
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));

            builder.Services.ConfigureCors();
            builder.Services.ConfigureSwagger();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(x =>
                {
                    x.SwaggerEndpoint("/swagger/v1/swagger.json", "CompanyEmployees API V1");
                    x.SwaggerEndpoint("/swagger/v2/swagger.json", "CompanyEmployee API V2");
                });
            }
            else
                app.UseHsts();

            app.UseHttpsRedirection();

            app.UseRateLimiter();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

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
