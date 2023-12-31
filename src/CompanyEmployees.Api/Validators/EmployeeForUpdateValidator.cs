using CompanyEmployees.Api.Models;
using FluentValidation;

namespace CompanyEmployees.Api.Validators;
public class EmployeeForUpdateValidator : AbstractValidator<EmployeeForUpdateDto>
{
    public EmployeeForUpdateValidator()
    {
        RuleFor(x => x.Name)
        .NotEmpty()
        .MaximumLength(30);

        RuleFor(x => x.Age).NotNull().GreaterThanOrEqualTo(18);

        RuleFor(x => x.Position)
        .NotEmpty()
        .MaximumLength(20);
    }
}
