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

    [HttpGet]
    [Authorize(Roles = "Manager,Adminstrator")]
    public async Task<IActionResult> GetEmployees([FromQuery] PaginationFilter pagination, [FromQuery] EmployeeParameters filter, Guid companyId)
    {
        var result = await _service.GetAsync(pagination, filter, companyId);
        return result.Match<IActionResult>
        (
            Ok,
            err => NotFound(err.ToProblemDetails())
        );
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Manager,Adminstrator")]
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
    [Authorize(Roles = "Adminstrator")]
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

    [HttpDelete("{id}")]
    [Authorize(Roles = "Adminstrator")]
    public async Task<IActionResult> DeleteEmployee(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Adminstrator")]
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

    [HttpPatch("{id}")]
    [Authorize(Roles = "Adminstrator")]
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
