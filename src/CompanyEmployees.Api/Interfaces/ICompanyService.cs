using CompanyEmployees.Api.Models;
using OneOf;
using OneOf.Types;

namespace CompanyEmployees.Api.Interfaces;
public interface ICompanyService
{
    public Task<IEnumerable<CompanyDto>> GetAllAsync(bool trackChanges, int count = 10);

    public Task<OneOf<CompanyDto, NotFound>> GetAsync(bool trackChanges, Guid id);
}
