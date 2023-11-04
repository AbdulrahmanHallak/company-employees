using System.Net.Http.Headers;
using CompanyEmployees.Api.Extensions;
using CompanyEmployees.Api.Interfaces;
using CompanyEmployees.Api.Models;
using CompanyEmployees.Api.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _service;
    private readonly IValidator<UserForRegisterationDto> _validator;
    public AuthenticationController(IAuthenticationService service, IValidator<UserForRegisterationDto> validator)
    {
        _service = service;
        _validator = validator;
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Signup(UserForRegisterationDto dto)
    {
        var validationResult = _validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return UnprocessableEntity(ModelState);
        }
        var result = await _service.RegisterUserAsync(dto);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.TryAddModelError(error.Code, error.Description);

            return BadRequest(ModelState);
        }

        return StatusCode(201);
    }
    [HttpPost]
    public async Task<IActionResult> Signin(UserForLoginDto dto)
    {
        var result = await _service.IsUserValidAsync(dto);
        if (!result)
            return Unauthorized();
        return Ok(new { Token = await _service.CreateTokenAsync() });
    }
}
