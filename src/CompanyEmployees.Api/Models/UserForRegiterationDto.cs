namespace CompanyEmployees.Api.Models;
public class UserForRegisterationDto
{
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string UserName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string? PhoneNumber { get; init; }
    public ICollection<string>? Roles { get; init; }

}
