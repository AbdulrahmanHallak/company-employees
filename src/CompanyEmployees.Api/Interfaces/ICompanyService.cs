using CompanyEmployees.Api.Errors;
using CompanyEmployees.Api.Models;
using OneOf;
using OneOf.Types;

namespace CompanyEmployees.Api.Interfaces;
public interface ICompanyService
{
    public Task<IEnumerable<CompanyDto>> GetAsync(int count = 10);

    public Task<OneOf<CompanyDto, NotFoundError>> GetAsync(Guid id);
    public Task<OneOf<IEnumerable<CompanyDto>, NotFoundCollectionError>> GetCollectionAsync(IEnumerable<Guid> ids);
    public Task<OneOf<CompanyDto, InternalServerError>> CreateAsync(CompanyForCreateDto dto);
    public Task DeleteAsync(Guid id);
    public Task<OneOf<Success, NotFoundError>> UpdateAsync(Guid id, CompanyForUpdateDto dto);
}
