using CompanyEmployees.Api.Errors;
using CompanyEmployees.Api.Models;
using CompanyEmployees.Api.RequestFeatures;
using OneOf;
using OneOf.Types;

namespace CompanyEmployees.Api.Interfaces;
public interface ICompanyService
{
    public Task<PaginatedList<CompanyDto>> GetAsync(PaginationFilter parameters);

    public Task<OneOf<CompanyDto, NotFoundError>> GetAsync(Guid id);
    public Task<OneOf<CompanyDto, InternalServerError>> CreateAsync(CompanyForCreateDto dto);
    public Task DeleteAsync(Guid id);
    public Task<OneOf<Success, NotFoundError>> UpdateAsync(Guid id, CompanyForUpdateDto dto);
}
