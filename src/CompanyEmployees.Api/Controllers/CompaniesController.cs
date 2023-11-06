using System.Text.RegularExpressions;
using CompanyEmployees.Api.Extensions;
using CompanyEmployees.Api.Interfaces;
using CompanyEmployees.Api.Models;
using CompanyEmployees.Api.RequestFeatures;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "v1")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _service;
    private readonly IValidator<CompanyForUpdateDto> _updateValidator;
    private readonly IValidator<CompanyForCreateDto> _createValidator;

    public CompaniesController
    (ICompanyService service
    , IValidator<CompanyForCreateDto> createValidator
    , IValidator<CompanyForUpdateDto> updateValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [Authorize(Roles = "Manager,Administrator")]
    public async Task<IActionResult> GetCompanies([FromQuery] PaginationFilter filter)
    {
        var companies = await _service.GetAsync(filter);
        return Ok(companies);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Manager,Administrator")]
    public async Task<IActionResult> GetCompany(Guid id)
    {
        var result = await _service.GetAsync(id);
        return result.Match<IActionResult>
        (
            Ok,
            err => NotFound(err.ToProblemDetails())
        );
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateCompany(CompanyForCreateDto dto)
    {
        var validation = _createValidator.Validate(dto);
        if (!validation.IsValid)
        {
            validation.AddToModelState(ModelState);
            return UnprocessableEntity(ModelState);
        }
        var result = await _service.CreateAsync(dto);
        return result.Match<IActionResult>
        (
            company => CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company),
            err => new ObjectResult(err.ToProblemDetails()) { StatusCode = 500 }
        );
    }
    // ? Is there a use case for this method
    // * if this method is exposed, a new POST endpoint must be created to handle creating a
    // * collection of companies.
    // ! if decided to remove this endpoint, the methods in the CompanyService should be removed as well.

    // [HttpGet("collection")]
    // public async Task<IActionResult> GetCompanyCollection(IEnumerable<Guid> ids)
    // {
    //     var result = await _service.GetCollectionAsync(ids);
    //     return result.Match<IActionResult>
    //     (
    //         Ok,
    //         err => NotFound(err.ToProblemDetails())
    //     );
    // }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> UpdateCompany(Guid id, CompanyForUpdateDto dto)
    {
        var validation = _updateValidator.Validate(dto);
        if (!validation.IsValid)
        {
            validation.AddToModelState(ModelState);
            return UnprocessableEntity(ModelState);
        }
        var result = await _service.UpdateAsync(id, dto);
        return result.Match<IActionResult>
        (
            success => NoContent(),
            err => NotFound(err.ToProblemDetails())
        );
    }
}
