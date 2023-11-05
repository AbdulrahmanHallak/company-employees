using CompanyEmployees.Api.Interfaces;
using CompanyEmployees.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TokenController : ControllerBase
{
    private readonly IAuthenticationService _service;
    public TokenController(IAuthenticationService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Refresh(TokenDto tokenDto)
    {
        var result = await _service.RefreshToken(tokenDto);
        return result.Match<IActionResult>
        (
            Ok,
            err => BadRequest(err.ToProblemDetails())
        );
    }
}
