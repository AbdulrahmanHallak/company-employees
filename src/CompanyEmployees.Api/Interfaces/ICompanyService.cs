using CompanyEmployees.Api.Models;

namespace CompanyEmployees.Api.Interfaces;
public interface ICompanyService
{
    public Task<IEnumerable<CompanyDto>> GetAllAsync(bool trackChanges, int count = 10);
}
