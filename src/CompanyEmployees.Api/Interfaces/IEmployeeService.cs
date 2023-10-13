using CompanyEmployees.Api.Models;
using OneOf.Types;
using OneOf;

namespace CompanyEmployees.Api.Interfaces;
public interface IEmployeeService
{
    public Task<OneOf<IEnumerable<EmployeeDto>, NotFound>> GetAsync(bool trackChanges, Guid companyId, int count = 10);
}
