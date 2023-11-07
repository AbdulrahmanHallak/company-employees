using CompanyEmployees.Api.Models;
using OneOf;
using CompanyEmployees.Api.Errors;
using OneOf.Types;
using CompanyEmployees.Api.Data.Entities;
using CompanyEmployees.Api.RequestFeatures;

namespace CompanyEmployees.Api.Interfaces;
public interface IEmployeeService
{
    /// <summary>
    /// Retrieves a paginated list of employees based on specified pagination, filtering parameters, and company ID.
    /// </summary>
    /// <param name="pagination">Pagination filter parameters for the list of employees.</param>
    /// <param name="filter">Filter parameters for fine-grained employee query.</param>
    /// <param name="companyId">The unique identifier of the company for which employees are being retrieved.</param>
    /// <returns>
    /// A <see cref="OneOf"/> type representing either a <see cref="PaginatedList{EmployeeDto}"/> on successful retrieval or a <see cref="NotFoundError"/>
    ///  if the specified company is not found.
    /// </returns>
    public Task<OneOf<PaginatedList<EmployeeDto>, NotFoundError>> GetAsync(PaginationFilter pagination, EmployeeParameters filter, Guid companyId);

    /// <summary>
    /// Retrieves details of a specific employee within a company based on their unique identifiers.
    /// </summary>
    /// <param name="companyId">The unique identifier of the company to which the employee belongs.</param>
    /// <param name="id">The unique identifier of the employee to retrieve.</param>
    /// <returns>
    /// A <see cref="OneOf"/> type representing either an <see cref="EmployeeDto"/> on successful retrieval or a
    /// <see cref="NotFoundError"/> if the specified company or employee is not found.
    /// </returns>
    public Task<OneOf<EmployeeDto, NotFoundError>> GetAsync(Guid companyId, Guid id);

    /// <summary>
    /// Creates a new employee for a specified company based on provided data.
    /// </summary>
    /// <param name="companyId">The unique identifier of the company for which the employee is being created.</param>
    /// <param name="dto">The data necessary to create the employee.</param>
    /// <returns>
    /// A <see cref="OneOf"/> type representing either an <see cref="EmployeeDto"/> on successful creation,
    /// a <see cref="NotFoundError"/> if the specified company is not found, or an
    /// <see cref="InternalServerError"/> if an error occurs during creation.
    /// </returns>
    public Task<OneOf<EmployeeDto, NotFoundError, InternalServerError>> CreateAsync(Guid companyId, EmployeeForCreateDto dto);

    /// <summary>
    /// Deletes an employee based on the specified ID.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to delete.</param>
    public Task DeleteAsync(Guid id);

    /// <summary>
    /// Updates details of a specific employee within a company based on their unique identifiers using provided data.
    /// </summary>
    /// <param name="companyId">The unique identifier of the company to which the employee belongs.</param>
    /// <param name="employeeId">The unique identifier of the employee to update.</param>
    /// <param name="dto">The data containing updated information for the employee.</param>
    /// <returns>
    /// A <see cref="OneOf"/> type representing either a <see cref="Success"/> on successful update or
    ///  a <see cref="NotFoundError"/> if the specified company or employee is not found.
    /// </returns>
    public Task<OneOf<Success, NotFoundError>> UpdateAsync(Guid companyId, Guid employeeId, EmployeeForUpdateDto dto);

    /// <summary>
    /// Retrieves an employee's details specifically for patching, based on their unique identifiers within a company.
    /// </summary>
    /// <param name="companyId">The unique identifier of the company to which the employee belongs.</param>
    /// <param name="id">The unique identifier of the employee for patching.</param>
    /// <returns>
    /// A <see cref="OneOf"/> type representing either an <see cref="EmployeeForUpdateDto"/>
    /// for patching or a <see cref="NotFoundError"/> if the specified company or employee is not found.
    /// </returns>
    /// <remarks>
    /// After the patching process, the method <see cref="SaveChangesForPatch"/> must be called to
    /// persist the changes.
    /// </remarks>
    public Task<OneOf<EmployeeForUpdateDto, NotFoundError>> GetForPatch(Guid companyId, Guid id);

    /// <summary>
    /// Saves changes made for patching an employee based on provided updates and the employee's unique identifier.
    /// </summary>
    /// <param name="dto">The updated data for the employee after patching.</param>
    /// <param name="id">The unique identifier of the employee.</param>
    /// <returns>
    /// A task representing the asynchronous process of saving changes made for patching the employee.
    /// </returns>
    /// <remarks>
    /// This method MUST be called after the call to <see cref="GetForPatch"/>
    /// </remarks>
    public Task SaveChangesForPatch(EmployeeForUpdateDto dto, Guid id);
}
