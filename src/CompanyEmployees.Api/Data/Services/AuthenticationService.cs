using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CompanyEmployees.Api.ConfigModels;
using CompanyEmployees.Api.Data.Entities;
using CompanyEmployees.Api.Errors;
using CompanyEmployees.Api.Interfaces;
using CompanyEmployees.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OneOf;

namespace CompanyEmployees.Api.Data.Services;
public class AuthenticationService : IAuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JwtSettings _jwtSettings;
    private User? _user;
    public AuthenticationService
    (ILogger<AuthenticationService> logger,
    RoleManager<IdentityRole> roleManager,
    UserManager<User> userManager,
    IOptions<JwtSettings> jwtSettings)
    {
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtSettings = jwtSettings.Value;
    }


    public async Task<IdentityResult> RegisterUserAsync(UserForRegisterationDto dto)
    {
        // TODO: check if the roles exist in the db
        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            UserName = dto.UserName,
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (result.Succeeded && !dto.Roles.IsNullOrEmpty())
        {
            await _userManager.AddToRolesAsync(user, dto.Roles!);
            _logger.LogInformation("User {UserId} registerd successfully and was assigned the "
            + "roles {@UserRoles}", user.Id, dto.Roles);
        }

        else if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Manager");
            _logger.LogInformation("User {UserId} registerd successfully and was assigned the "
            + "roles {@UserRoles}", user.Id, "Manager");
        }
        else
            _logger.LogInformation("Failed to register user {UserName} with email {UserEmail} due "
            + "to the following errors {RegisterationErrors}", dto.FirstName + ' ' + dto.LastName, dto.Email, result.Errors);

        return result;
    }

    public async Task<bool> IsUserValidAsync(UserForLoginDto dto)
    {
        _user = await _userManager.FindByNameAsync(dto.UserName);
        if (_user is null)
        {
            _logger.LogWarning("Login attempt to the {Username} failed due to being not registered", dto.UserName);
            return false;
        }
        var result = await _userManager.CheckPasswordAsync(_user, dto.Password);
        if (!result)
            _logger.LogWarning("Login attempt to the user {Username} failed due to invalid password", dto.UserName);

        return result;
    }
    public async Task<TokenDto> CreateTokenAsync(bool publateExp)
    {
        var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET")!);
        var secret = new SymmetricSecurityKey(key);
        var signingCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        var expiryDate = DateTime.Now.AddMinutes(_jwtSettings.Expires);


        // ? It seems the ClaimType.Name is required by Identity
        // ? to validate associate with the user.
        // ? Properly test this behaviour and see the source code
        // ? to verify this.
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, _user!.UserName!),
            new Claim(ClaimTypes.Name , _user!.UserName!),
            new Claim(JwtRegisteredClaimNames.Email, _user.Email!),
            new Claim(JwtRegisteredClaimNames.Iss, _jwtSettings.ValidIssuer),
            new Claim(JwtRegisteredClaimNames.Exp, expiryDate.ToString())
        };
        var roles = await _userManager.GetRolesAsync(_user);
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var tokenOptions = new JwtSecurityToken
        (
            audience: _jwtSettings.ValidAudience,
            issuer: _jwtSettings.ValidIssuer,
            claims: claims,
            expires: expiryDate,
            signingCredentials: signingCredentials
        );

        var refreshToken = GenerateRefreshToken();
        _user.RefreshToken = refreshToken;

        if (publateExp)
            _user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

        await _userManager.UpdateAsync(_user);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return new TokenDto() { AccessToken = accessToken, RefreshToken = refreshToken };
    }

    public async Task<OneOf<TokenDto, InvalidTokenError>> RefreshToken(TokenDto tokenDto)
    {
        var result = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

        if (result.IsT1)
            return result.AsT1;

        var principal = result.AsT0;
        var user = await _userManager.FindByNameAsync(principal.Identity!.Name!);
        if (user is null || user.RefreshToken != tokenDto.RefreshToken
            || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return new InvalidTokenError("Invalid Refresh Token");

        _user = user;

        return await CreateTokenAsync(false);
    }
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    private OneOf<ClaimsPrincipal, InvalidTokenError> GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameter = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false, // Because it is already expired. // ! if this set to true a SecurityTokenExpiredException is thrown
            ValidAudience = _jwtSettings.ValidAudience,                 // ! by ValidateToken method.
            ValidIssuer = _jwtSettings.ValidIssuer,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET")!))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken validToken;
        ClaimsPrincipal principal;
        try
        {
            principal = tokenHandler.ValidateToken(token, tokenValidationParameter, out validToken);
        }
        catch (Exception)
        {
            return new InvalidTokenError("Invalid access token");
        }

        var jwtSecurityToken = validToken as JwtSecurityToken;

        if (jwtSecurityToken is null ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            return new InvalidTokenError("Invalid Access Token");

        return principal;
    }
}