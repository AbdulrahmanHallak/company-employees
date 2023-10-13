using CompanyEmployees.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Namespace;

[ApiController]
[Route("api/companies/{companyId}/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetEmployeesAsync(Guid companyId)
    {
        var result = await _service.GetAsync(false, companyId);
        return result.Match<IActionResult>
        (
            Ok,
            _ => NotFound($"There is no company with the provided {companyId}")
        );
    }

}
