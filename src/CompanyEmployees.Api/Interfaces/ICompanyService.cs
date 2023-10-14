using CompanyEmployees.Api.Models;
using OneOf;
using OneOf.Types;

namespace CompanyEmployees.Api.Interfaces;
public interface ICompanyService
{
    public Task<IEnumerable<CompanyDto>> GetAsync(int count = 10);

    public Task<OneOf<CompanyDto, NotFound>> GetAsync(Guid id);
}
