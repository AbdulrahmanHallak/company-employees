using CompanyEmployees.Api.Interfaces;
using CompanyEmployees.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Namespace;

[ApiController]
[Route("api/companies/{companyId}/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetEmployees(Guid companyId)
    {
        var result = await _service.GetAsync(companyId);
        return result.Match<IActionResult>
        (
            Ok,
            err => NotFound(err.ToProblemDetails())
        );
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployee(Guid companyId, Guid id)
    {
        var result = await _service.GetAsync(companyId, id);
        return result.Match<IActionResult>
        (
            Ok,
            err => NotFound(err.ToProblemDetails())
        );
    }

    [HttpPost]
    public async Task<IActionResult> CreatEmployee(Guid companyId, EmployeeForCreateDto dto)
    {
        var result = await _service.CreateAsync(companyId, dto);
        return result.Match
        (
            dto => CreatedAtAction(nameof(GetEmployee), new { companyId, id = dto.Id }, dto),
            notFoundError => NotFound(notFoundError.ToProblemDetails()),
            internalServerError => new ObjectResult(internalServerError.ToProblemDetails()) { StatusCode = 500 }
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(Guid companyId, Guid id, EmployeeForUpdateDto dto)
    {
        var result = await _service.UpdateEmployeeAsync(companyId, id, dto);
        return result.Match<IActionResult>
        (
            success => NoContent(),
            err => NotFound(err.ToProblemDetails())
        );
    }
}
