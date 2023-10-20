using CompanyEmployees.Api.Models;
using OneOf;
using CompanyEmployees.Api.Errors;

namespace CompanyEmployees.Api.Interfaces;
public interface IEmployeeService
{
    public Task<OneOf<IEnumerable<EmployeeDto>, NotFoundError>> GetAsync(Guid companyId, int count = 10);

    public Task<OneOf<EmployeeDto, NotFoundError>> GetAsync(Guid companyId, Guid id);

    public Task<OneOf<EmployeeDto, NotFoundError, InternalServerError>> CreateAsync(Guid companyId, EmployeeForCreateDto dto);

    public Task DeleteAsync(Guid id);
}
