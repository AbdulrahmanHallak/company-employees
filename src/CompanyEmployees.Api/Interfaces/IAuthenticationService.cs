using CompanyEmployees.Api.Errors;
using CompanyEmployees.Api.Models;
using Microsoft.AspNetCore.Identity;
using OneOf;

namespace CompanyEmployees.Api.Interfaces;
public interface IAuthenticationService
{
    public Task<IdentityResult> RegisterUserAsync(UserForRegisterationDto dto);
    public Task<bool> IsUserValidAsync(UserForLoginDto dto);
    public Task<TokenDto> CreateTokenAsync(bool populateExp);
    public Task<OneOf<TokenDto, InvalidTokenError>> RefreshToken(TokenDto tokenDto);
}
