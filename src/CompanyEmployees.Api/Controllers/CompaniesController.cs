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

    /// <summary>
    /// Retrieves a paginated list of companies based on the provided pagination filter.
    /// </summary>
    /// <param name="filter">The pagination filter containing parameters for configuring  pagination.</param>
    /// <response code="201">Returns the paginated list of companies on successful retrieval.</response>
    /// <response code="401">Unauthorized - returned when the user is not authorized.</response>
    [HttpGet]
    [Authorize(Roles = "Manager,Administrator")]
    [ProducesResponseType(typeof(PaginatedList<CompanyDto>), 201)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetCompanies([FromQuery] PaginationFilter filter)
    {
        var companies = await _service.GetAsync(filter);
        return Ok(companies);
    }

    /// <summary>
    /// Retrieves the company identified by the provided Id.
    /// </summary>
    /// <param name="id">The unique identifier of the company to be retrieved.</param>
    /// <response code="200">Returns the company successfully retrieved by its ID.</response>
    /// <response code="404">Not Found - returned when the specified company is not found.</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Manager,Administrator")]
    [ProducesResponseType(typeof(CompanyDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCompany(Guid id)
    {
        var result = await _service.GetAsync(id);
        return result.Match<IActionResult>
        (
            Ok,
            err => NotFound(err.ToProblemDetails())
        );
    }


    /// <summary>
    /// Creates a new company based on the provided data.
    /// </summary>
    /// <param name="dto">The data necessary to create the company.</param>
    /// <response code="201">Returns the newly created company along with its URI in the Location header.</response>
    /// <response code="422">Unprocessable Entity - returned when the provided data is invalid.</response>
    /// <response code="500">Internal Server Error - returned if an error occurs during company creation.</response>
    [HttpPost]
    [Authorize(Roles = "Administrator")]
    [ProducesResponseType(typeof(CompanyDto), 201)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
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

    /// <summary>
    /// Deletes the company identified by the provided ID.
    /// </summary>
    /// <param name="id">The unique identifier of the company to be deleted.</param>
    /// <response code="204">No Content - returned upon successful deletion of the specified company.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }


    /// <summary>
    /// Updates the details of the company identified by the provided ID.
    /// </summary>
    /// <param name="id">The unique identifier of the company to be updated.</param>
    /// <param name="dto">The data containing updated information for the company.</param>
    /// <response code="204">No Content - returned upon successful update of the specified company.</response>
    /// <response code="422">Unprocessable Entity - returned when the provided data is invalid.</response>
    /// <response code="404">Not Found - returned when the specified company is not found.</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator")]
    [ProducesResponseType(204)]
    [ProducesResponseType(422)]
    [ProducesResponseType(404)]
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
