using CompanyEmployees.Api.Data.Models;

namespace CompanyEmployees.Api.Interfaces;
public interface ICompanyService
{
    public Task<IEnumerable<Company>> GetAllAsync(bool trackChanges, int count = 10);
}
