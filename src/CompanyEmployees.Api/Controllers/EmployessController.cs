using CompanyEmployees.Api.Errors;
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
    public async Task<IActionResult> GetEmployeesAsync(Guid companyId)
    {
        var result = await _service.GetAsync(companyId);
        return result.Match<IActionResult>
        (
            Ok,
            NotFound
        );
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployeeAsync(Guid companyId, Guid id)
    {
        var result = await _service.GetAsync(companyId, id);
        return result.Match<IActionResult>
        (
            Ok,
            NotFound
        );
    }

    [HttpPost]
    public async Task<IActionResult> CreatEmployeeAsync(Guid companyId, EmployeeForCreateDto dto)
    {
        var result = await _service.CreateAsync(companyId, dto);
        return result.Match
        (
            dto => CreatedAtAction("GetEmployee", new { companyId, id = dto.Id }, dto),
            NotFound,
            err => new ObjectResult(err) { StatusCode = 500 }
        );
    }
}
