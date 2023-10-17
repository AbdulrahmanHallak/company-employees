using CompanyEmployees.Api.Interfaces;
using CompanyEmployees.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OneOf.Types;

namespace CompanyEmployees.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _service;

    public CompaniesController(ICompanyService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetCompanies()
    {
        var companies = await _service.GetAsync();
        return Ok(companies);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCompany(Guid id)
    {
        var result = await _service.GetAsync(id);
        return result.Match<IActionResult>
        (
            Ok,
            NotFound
        );
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompany(CompanyForCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return result.Match<IActionResult>
        (
            company => CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company),
            err => new ObjectResult(err) { StatusCode = 500 }
        );
    }
}
