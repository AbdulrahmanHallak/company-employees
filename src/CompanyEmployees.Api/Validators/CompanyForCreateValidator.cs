using CompanyEmployees.Api.Models;
using FluentValidation;

namespace CompanyEmployees.Api.Validators;
public class CompanyForCreateValidator : AbstractValidator<CompanyForCreateDto>
{
    public CompanyForCreateValidator()
    {
        RuleFor(x => x.Name)
        .NotEmpty()
        .MaximumLength(60);

        RuleFor(x => x.Address)
        .NotEmpty()
        .MaximumLength(60);

        RuleFor(x => x.Country)
        .NotEmpty()
        .MaximumLength(40);

        RuleFor(x => x.Employees)
        .ForEach(x => x.SetValidator(new EmployeeForCreateValidator()));
    }
}
