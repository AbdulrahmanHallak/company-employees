using CompanyEmployees.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace CompanyEmployees.Api.Interfaces;
public interface IAuthenticationService
{
    public Task<IdentityResult> RegisterUserAsync(UserForRegisterationDto dto);
    public Task<bool> IsUserValidAsync(UserForLoginDto dto);
    public Task<string> CreateTokenAsync();
}
