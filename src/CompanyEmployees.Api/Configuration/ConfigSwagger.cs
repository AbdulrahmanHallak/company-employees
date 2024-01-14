using System.Reflection;
using CompanyEmployees.Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace CompanyEmployees.Api.Configuration;
public static class ConfigSwagger
{
    public static IServiceCollection ConfigureSwagger(this IServiceCollection service)
    {
        service.AddSwaggerGen(x =>
        {
            x.SupportNonNullableReferenceTypes();
            x.SchemaFilter<SwaggerFluentValidation>();
            x.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CompanyEmployees API V1",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "Abdulrahman Hallak",
                    Email = "abdulrahmanhallak04@gmail.com",
                    Url = new Uri("https://github.com/AbdulrahmanHallak")
                }
            });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.docs.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            x.IncludeXmlComments(xmlPath);
            x.SwaggerDoc("v2", new OpenApiInfo
            {
                Title = "CompanyEmployee API V2",
                Version = "v2",
                Contact = new OpenApiContact
                {
                    Name = "Abdulrahman Hallak",
                    Email = "abdulrahmanhallak04@gmail.com",
                    Url = new Uri("https://github.com/AbdulrahmanHallak")
                }
            });

            x.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Place to add JWT with bearer",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });

            x.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            },
                        Name = JwtBearerDefaults.AuthenticationScheme
                        }, new List<string>()
                    }
            });
        });
        return service;
    }
}
