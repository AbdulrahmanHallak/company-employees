using CompanyEmployees.Api.Errors;
using CompanyEmployees.Api.Models;
using CompanyEmployees.Api.RequestFeatures;
using OneOf;
using OneOf.Types;

namespace CompanyEmployees.Api.Interfaces;
public interface ICompanyService
{
    /// <summary>
    /// Retrieves a paginated list of companies based on the provided pagination parameters.
    /// </summary>
    /// <param name="parameters">Pagination filter parameters for the list of companies.</param>
    /// <returns>
    /// A <see cref="PaginatedList{CompanyDto}"/> containing <see cref="CompanyDto"/>
    /// </returns>
    public Task<PaginatedList<CompanyDto>> GetAsync(PaginationFilter parameters);

    /// <summary>
    /// Retrieves details of a specific company based on its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the company to retrieve.</param>
    /// <returns>
    /// A <see cref="OneOf"/> type representing either an <see cref="CompanyDto"/> on successful retrieval or a
    /// <see cref="NotFoundError"/> if the specified company is not found.
    /// </returns>
    public Task<OneOf<CompanyDto, NotFoundError>> GetAsync(Guid id);

    /// <summary>
    /// Creates a new company based on provided data.
    /// </summary>
    /// <param name="dto">The data necessary to create the company.</param>
    /// <returns>
    /// A <see cref="OneOf"/> representing either a <see cref="CompanyDto"/> on successful creation or an
    /// <see cref="InternalServerError"/> in case of an error during creation.
    /// </returns>
    public Task<OneOf<CompanyDto, InternalServerError>> CreateAsync(CompanyForCreateDto dto);

    /// <summary>
    /// Deletes a company based on the specified ID.
    /// </summary>
    /// <param name="id">The unique identifier of the company to delete.</param>
    public Task DeleteAsync(Guid id);
    /// <summary>
    /// Updates details of a specific company based on its unique identifier using provided data.
    /// </summary>
    /// <param name="id">The unique identifier of the company to update.</param>
    /// <param name="dto">The data containing updated information for the company.</param>
    /// <returns>
    /// A <see cref="OneOf"/> representing either a <see cref="Success"/> on successful update
    /// or a <see cref="NotFoundError"/> if the specified company is not found.
    /// </returns>
    public Task<OneOf<Success, NotFoundError>> UpdateAsync(Guid id, CompanyForUpdateDto dto);
}
