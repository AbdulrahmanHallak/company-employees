using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CompanyEmployees.Api.Configuration;
using CompanyEmployees.Api.Data.Entities;
using CompanyEmployees.Api.Interfaces;
using CompanyEmployees.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace CompanyEmployees.Api.Services;
public class AuthenticationService : IAuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private User? _user;
    public AuthenticationService
    (ILogger<AuthenticationService> logger,
    RoleManager<IdentityRole> roleManager,
    UserManager<User> userManager,
    IConfiguration configuration)
    {
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
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
    public async Task<string> CreateTokenAsync()
    {
        var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET")!);
        var secret = new SymmetricSecurityKey(key);
        var signingCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        var jwtSettings = _configuration.GetSection("JwtSettings");


        // ? It seems the ClaimType.Name is required by Identity
        // ? to validate associate with the user.
        // ? Properly test this behaviour and see the source code
        // ? to verify this.
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, _user!.UserName!),
            new Claim(ClaimTypes.Name , _user!.UserName!),
            new Claim(JwtRegisteredClaimNames.Email, _user.Email!),
            new Claim(JwtRegisteredClaimNames.Iss, jwtSettings["validIssuer"]!),
        };
        var roles = await _userManager.GetRolesAsync(_user);
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var tokenOptions = new JwtSecurityToken
        (
            audience: jwtSettings["validAudience"],
            issuer: jwtSettings["validIssuer"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["expires"]!)),
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
}
