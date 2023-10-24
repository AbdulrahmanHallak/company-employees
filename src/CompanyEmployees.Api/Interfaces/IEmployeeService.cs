using CompanyEmployees.Api.Models;
using OneOf;
using CompanyEmployees.Api.Errors;
using OneOf.Types;
using CompanyEmployees.Api.Data.Entities;
using CompanyEmployees.Api.RequestFeatures;

namespace CompanyEmployees.Api.Interfaces;
public interface IEmployeeService
{
    public Task<OneOf<PaginatedList<EmployeeDto>, NotFoundError>> GetAsync(PaginationFilter pagination,EmployeeFilter filter, Guid companyId);

    public Task<OneOf<EmployeeDto, NotFoundError>> GetAsync(Guid companyId, Guid id);

    public Task<OneOf<EmployeeDto, NotFoundError, InternalServerError>> CreateAsync(Guid companyId, EmployeeForCreateDto dto);

    public Task DeleteAsync(Guid id);

    public Task<OneOf<Success, NotFoundError>> UpdateAsync(Guid companyId, Guid employeeId, EmployeeForUpdateDto dto);

    public Task<OneOf<EmployeeForUpdateDto, NotFoundError>> GetForPatch(Guid companyId, Guid id);
    public Task SaveChangesForPatch(EmployeeForUpdateDto dto, Guid id);
}
