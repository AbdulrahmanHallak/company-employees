using CompanyEmployees.Api.Data.Entities;

namespace CompanyEmployees.Api.Interfaces;
public interface ICompanyService
{
    public Task<IEnumerable<Company>> GetAllAsync(bool trackChanges, int count = 10);
}
