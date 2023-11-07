using FluentValidation;
using FluentValidation.Validators;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CompanyEmployees.Api.Extensions;
public class SwaggerFluentValidation : ISchemaFilter
{
    private readonly IServiceProvider _provider;

    public SwaggerFluentValidation(IServiceProvider provider) => _provider = provider;
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var validator = _provider.GetService(typeof(IValidator<>).MakeGenericType(context.Type)) as IValidator;

        if (validator is null)
            return;
        if (schema.Required is null)
            schema.Required = new HashSet<string>();

        var validatorDescriptor = validator.CreateDescriptor();

        foreach (var key in schema.Properties.Keys)
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
    private static string? ToPascalCase(string inputString)
    {
        if (string.IsNullOrEmpty(inputString) || inputString.Length < 2)
        {
            return null;
        }
        return inputString.Substring(0, 1).ToUpper() + inputString.Substring(1);
    }
}
