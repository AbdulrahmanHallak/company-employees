using Microsoft.AspNetCore.Identity;

namespace CompanyEmployees.Api.Data.Entities;
public class User : IdentityUser
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? RefreshToken { get; set; } = default!;
    public DateTime RefreshTokenExpiryTime { get; set; }
}
