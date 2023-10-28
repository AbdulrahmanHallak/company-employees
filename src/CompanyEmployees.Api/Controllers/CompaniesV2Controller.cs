using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Api.Controllers;

[ApiController]
[Route("api/companies")]
public class CompaniesV2Controller : ControllerBase
{
    public CompaniesV2Controller()
    {
    }

    [HttpGet]
    public IActionResult GetCompanies()
    {
        return Ok("Testing Versioning");
    }
}
