using CompanyEmployees.Api.Models;
using FluentValidation;

namespace CompanyEmployees.Api.Validators;
public class UserForRegisterValidator : AbstractValidator<UserForRegisterationDto>
{
    public UserForRegisterValidator()
    {
        RuleFor(x => x.Email)
        .NotEmpty()
        .Matches($@"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$");

        RuleFor(x => x.UserName)
        .NotEmpty()
        .Matches($@"^[a-zA-Z0-9\-._@+]+$");

        RuleFor(x => x.FirstName).NotEmpty();

        RuleFor(x => x.LastName).NotEmpty();

        RuleFor(x => x.Password).NotEmpty();

        RuleFor(x => x.PhoneNumber).Matches(@$"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$");
    }
}
