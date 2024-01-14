using FluentValidation;
using FluentValidation.Validators;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CompanyEmployees.Api.Configuration;

/// <summary>
/// Swagger validation documentation with FluentValidations.
/// See <seealso cref="ISchemaFilter" />
/// </summary>
public class SwaggerFluentValidation : ISchemaFilter
{
    private readonly IServiceScopeFactory _scopeFactory;

    public SwaggerFluentValidation(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        using IServiceScope provider = _scopeFactory.CreateScope();
        var validator = provider.ServiceProvider.GetRequiredService(typeof(IValidator<>).MakeGenericType(context.Type)) as IValidator;

        if (validator is null)
            return;

        if (schema.Required is null)
            schema.Required = new HashSet<string>();

        var validatorDescriptor = validator.CreateDescriptor();

        foreach (string key in schema.Properties.Keys)
        {
            foreach (var propertyValidator in validatorDescriptor.GetValidatorsForMember(ToPascalCase(key)))
            {
                if (propertyValidator.Validator is INotNullValidator or INotEmptyValidator)
                    schema.Required.Add(key);

                if (propertyValidator.Validator is ILengthValidator lengthValidator)
                {
                    if (lengthValidator.Max > 0)
                        schema.Properties[key].MaxLength = lengthValidator.Max;

                    schema.Properties[key].MinLength = lengthValidator.Min;
                }

                if (propertyValidator.Validator is IRegularExpressionValidator regularExpressionValidator)
                {
                    schema.Properties[key].Pattern = regularExpressionValidator.Expression;
                }
            }
        }
    }
    /// <summary>
    /// To convert case as swagger may be using lower camel case.
    /// </summary>
    /// <param name="inputString">The input string.</param>
    /// <returns>Pascal case for string.</returns>
    private static string? ToPascalCase(string inputString)
    {
        if (string.IsNullOrEmpty(inputString) || inputString.Length < 2)
        {
            return null;
        }
        return inputString.Substring(0, 1).ToUpper() + inputString.Substring(1);
    }
}
