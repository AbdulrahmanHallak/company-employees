using CompanyEmployees.Api.Extensions;
using CompanyEmployees.Api.Interfaces;
using CompanyEmployees.Api.Models;
using CompanyEmployees.Api.RequestFeatures;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Api.Controllers;

[ApiController]
[Route("api/companies/{companyId}/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;
    private readonly IValidator<EmployeeForCreateDto> _createValidator;
    private readonly IValidator<EmployeeForUpdateDto> _updateValidator;

    public EmployeesController
    (IEmployeeService service
    , IValidator<EmployeeForUpdateDto> updateValidator
    , IValidator<EmployeeForCreateDto> createValidator)
    {
        _service = service;
        _updateValidator = updateValidator;
        _createValidator = createValidator;
    }

    /// <summary>
    /// Retrieves a paginated list of employees for a specified company using pagination and filtering parameters.
    /// </summary>
    /// <param name="pagination">Pagination filter parameters for the list of employees.</param>
    /// <param name="filter">Filter parameters for fine-grained employee query.</param>
    /// <param name="companyId">The unique identifier of the company for which employees are being retrieved.</param>
    /// <response code="200">Returns the paginated list on successful retrieval.</response>
    /// <response code="404">Not Found - returned when the specified company is not found.</response>
    [HttpGet]
    [Authorize(Roles = "Manager,Adminstrator")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetEmployees([FromQuery] PaginationFilter pagination, [FromQuery] EmployeeParameters filter, Guid companyId)
    {
        var result = await _service.GetAsync(pagination, filter, companyId);
        return result.Match<IActionResult>
        (
            Ok,
            err => NotFound(err.ToProblemDetails())
        );
    }

    /// <summary>
    /// Retrieves a specific employee within a company based on their unique identifiers.
    /// </summary>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="id">The unique identifier of the employee.</param>
    /// <response code="200">Returns the retrieved employee within the specified company.</response>
    /// <response code="404">Not Found - returned when the specified company or employee is not found.</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Manager,Adminstrator")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetEmployee(Guid companyId, Guid id)
    {
        var result = await _service.GetAsync(companyId, id);
        return result.Match<IActionResult>
        (
            Ok,
            err => NotFound(err.ToProblemDetails())
        );
    }

    /// <summary>
    /// Creates a new employee for a specified company based on provided data.
    /// </summary>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="dto">The data necessary to create the employee.</param>
    /// <response code="201">Returns the newly created employee with its URI in the Location header.</response>
    /// <response code="422">Unprocessable Entity - returned when the provided data is invalid.</response>
    /// <response code="404">Not Found - returned when the specified company is not found.</response>
    /// <response code="500">Internal Server Error - returned if an error occurs during employee creation.</response>
    [HttpPost]
    [Authorize(Roles = "Adminstrator")]
    [ProducesResponseType(201)]
    [ProducesResponseType(422)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CreatEmployee(Guid companyId, EmployeeForCreateDto dto)
    {
        var validation = _createValidator.Validate(dto);
        if (!validation.IsValid)
        {
            validation.AddToModelState(ModelState);
            return UnprocessableEntity(ModelState);
        }
        var result = await _service.CreateAsync(companyId, dto);
        return result.Match
        (
            dto => CreatedAtAction(nameof(GetEmployee), new { companyId, id = dto.Id }, dto),
            notFoundError => NotFound(notFoundError.ToProblemDetails()),
            internalServerError => new ObjectResult(internalServerError.ToProblemDetails()) { StatusCode = 500 }
        );
    }

    /// <summary>
    /// Deletes an employee based on the specified ID.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to be deleted.</param>
    /// <response code="204">No Content - returned upon successful deletion of the specified employee.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Adminstrator")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteEmployee(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Updates details of a specific employee within a company based on their unique identifiers.
    /// </summary>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="id">The unique identifier of the employee.</param>
    /// <param name="dto">The data containing updated information for the employee.</param>
    /// <response code="204">No Content - returned upon successful update of the specified employee.</response>
    /// <response code="422">Unprocessable Entity - returned when the provided data is invalid.</response>
    /// <response code="404">Not Found - returned when the specified company or employee is not found.</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Adminstrator")]
    [ProducesResponseType(204)]
    [ProducesResponseType(422)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateEmployee(Guid companyId, Guid id, EmployeeForUpdateDto dto)
    {
        var validation = _updateValidator.Validate(dto);
        if (!ModelState.IsValid)
        {
            validation.AddToModelState(ModelState);
            return UnprocessableEntity(ModelState);
        }
        var result = await _service.UpdateAsync(companyId, id, dto);
        return result.Match<IActionResult>
        (
            success => NoContent(),
            err => NotFound(err.ToProblemDetails())
        );
    }

    /// <summary>
    /// Updates specific details of an employee within a company by applying partial changes using JSON Patch operations.
    /// </summary>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="id">The unique identifier of the employee.</param>
    /// <param name="pathDoc">JSON Patch Document containing partial updates for the employee.</param>
    /// <response code="204">No Content - returned upon successful application of partial updates to the specified employee.</response>
    /// <response code="422">Unprocessable Entity - returned when the provided data in the patch document is invalid.</response>
    /// <response code="404">Not Found - returned when the specified company or employee is not found.</response>
    [HttpPatch("{id}")]
    [Authorize(Roles = "Adminstrator")]
    [ProducesResponseType(204)]
    [ProducesResponseType(422)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> PatchEmployee(Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> pathDoc)
    {
        var result = await _service.GetForPatch(companyId, id);

        if (result.IsT1)
            return NotFound(result.AsT1.ToProblemDetails());

        pathDoc.ApplyTo(result.AsT0, ModelState);
        var validation = _updateValidator.Validate(result.AsT0);
        if (!validation.IsValid)
        {
            validation.AddToModelState(ModelState);
            return UnprocessableEntity(ModelState);
        }

        await _service.SaveChangesForPatch(result.AsT0, id);

        return NoContent();
    }
}
