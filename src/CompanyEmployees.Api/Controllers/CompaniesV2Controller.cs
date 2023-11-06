using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Api.Controllers;

[ApiController]
[Route("api/companies")]
[ApiExplorerSettings(GroupName = "v2")]
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
