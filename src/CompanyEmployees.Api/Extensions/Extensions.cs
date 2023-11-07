using Asp.Versioning;
using CompanyEmployees.Api.Controllers;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CompanyEmployees.Api.Extensions;
public static class Extensions
{
    public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
    {
        foreach (var error in result.Errors)
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
    }
    public static IServiceCollection ConfigureVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(opts =>
        {
            opts.AssumeDefaultVersionWhenUnspecified = true;
            opts.ReportApiVersions = true;
            opts.DefaultApiVersion = new ApiVersion(1, 0);
            opts.ApiVersionReader = new HeaderApiVersionReader("api-version");
        }).AddMvc(opts =>
        {
            opts.Conventions.Controller<CompaniesController>().HasApiVersion(new ApiVersion(1, 0));
            opts.Conventions.Controller<CompaniesV2Controller>().HasDeprecatedApiVersion(new ApiVersion(2, 0));
            opts.Conventions.Controller<EmployeesController>().HasApiVersion(new ApiVersion(1, 0));
        });
        return services;
    }
}
