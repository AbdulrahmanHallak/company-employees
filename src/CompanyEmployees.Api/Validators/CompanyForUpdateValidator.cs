using CompanyEmployees.Api.Models;
using FluentValidation;

namespace CompanyEmployees.Api.Validators;
public class CompanyForUpdateValidator : AbstractValidator<CompanyForUpdateDto>
{
    public CompanyForUpdateValidator()
    {
        RuleFor(x => x.Name)
        .NotNull()
        .MaximumLength(60);

        RuleFor(x => x.Address)
        .NotNull()
        .MaximumLength(60);

        RuleFor(x => x.Country)
        .NotNull()
        .MaximumLength(40);

        RuleFor(x => x.Employees)
        .ForEach(x => x.SetValidator(new EmployeeForCreateValidator()));
    }
}
