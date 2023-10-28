using CompanyEmployees.Api.Data;
using CompanyEmployees.Api.Data.Entities;
using CompanyEmployees.Api.Errors;
using CompanyEmployees.Api.Interfaces;
using CompanyEmployees.Api.Models;
using CompanyEmployees.Api.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using CompanyEmployees.Api.Extensions;
using OneOf;
using OneOf.Types;

namespace CompanyEmployees.Api.Services;
public class EmployeeService : IEmployeeService
{
    private readonly ILogger<EmployeeService> _logger;
    private readonly AppDbContext _context;

    public EmployeeService(ILogger<EmployeeService> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<OneOf<PaginatedList<EmployeeDto>, NotFoundError>> GetAsync(PaginationFilter? pagination, EmployeeParameters filter, Guid companyId)
    {
        if (pagination is null) pagination = new PaginationFilter();

        var company = await _context.Companies.FindAsync(companyId);
        if (company is null)
        {
            _logger.LogWarning("A request to retrieve employees of a company with a non-exsistent id {CompanyId}", companyId);
            return new NotFoundError(message: "There is no company with the provided id", id: companyId.ToString());
        }

        // The actual query.
        var employees =
           _context.Employees.AsNoTracking()
           .FilterByAge(filter.MinAge, filter.MaxAge)
           .SearchByName(filter.SearchTerm!)
           .Sort(pagination.OrderBy)
           .Select(x => new EmployeeDto() { Id = x.Id, Name = x.Name, Age = x.Age, Position = x.Position });

        return await PaginatedList<EmployeeDto>.CreateAsync(employees, pagination.PageNumber, pagination.PageSize);
    }

    public async Task<OneOf<EmployeeDto, NotFoundError>> GetAsync(Guid companyId, Guid id)
    {
        // Check if the company exists.
        var company = await _context.Companies.FindAsync(companyId);
        if (company is null)
        {
            _logger.LogWarning("A request to retrieve employees of a company with a non-exsistent id {ComopanyId}", companyId);
            return new NotFoundError(message: "There is no company with the provided Id", id: companyId.ToString());
        }

        EmployeeDto? employee;
        employee = await
            (from emp in _context.Employees.AsNoTracking()
             where emp.Id == id
             select new EmployeeDto()
             {
                 Id = emp.Id,
                 Name = emp.Name,
                 Age = emp.Age,
                 Position = emp.Position
             }).SingleOrDefaultAsync();

        if (employee is null)
        {
            _logger.LogWarning("A request to retrieve a employee with a non exsistent id {EmployeeId}", id);
            return new NotFoundError(message: "There is no employee with the provided Id", id: id.ToString());
        }
        else return employee;
    }

    public async Task<OneOf<EmployeeDto, NotFoundError, InternalServerError>> CreateAsync(Guid companyId, EmployeeForCreateDto dto)
    {
        // Check if the company exists.
        var company = await _context.Companies.FindAsync(companyId);
        if (company is null)
        {
            _logger.LogWarning("A request to create an employee entity for a company with a non-exsistent id {CompanyId}", companyId);
            return new NotFoundError(message: "There is no company with the provided Id", id: companyId.ToString());
        }
        // Map to entity.
        var employee = new Employee()
        {
            Name = dto.Name,
            Age = dto.Age,
            Position = dto.Position,
            CompanyId = companyId
        };

        _context.Employees.Add(employee);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("the following exception was thrown when saving {@Entity}: {Exception}", employee, ex);
            return new InternalServerError("An error occurred while processing the request. Please contact support");
        }
        return new EmployeeDto()
        {
            Id = employee.Id,
            Name = employee.Name,
            Age = employee.Age,
            Position = employee.Position,
        };
    }

    public async Task DeleteAsync(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee is null)
        {
            _logger.LogWarning("Request to delete a non-existent employee with the following id: {EmployeeId}", id);
            return;
        }

        _context.Remove(employee);
        await _context.SaveChangesAsync();
    }

    public async Task<OneOf<Success, NotFoundError>> UpdateAsync(Guid companyId, Guid employeeId, EmployeeForUpdateDto dto)
    {
        // Check if company and employee exist:
        var company = await _context.Companies.FindAsync(companyId);
        if (company is null)
        {
            _logger.LogWarning("A request to update a company with a non exsistent id {CompanyId}", companyId);
            return new NotFoundError("There is no company with the provided id", companyId.ToString());
        }
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee is null)
        {
            _logger.LogWarning("A request to update an employee with a non exsistent id {EmployeeId}", employeeId);
            return new NotFoundError(message: "There is no employee with the provided Id", id: employeeId.ToString());
        }

        // mapping to entity:
        employee.Position = dto.Position;
        employee.Name = dto.Name;
        employee.Age = dto.Age;

        await _context.SaveChangesAsync();

        return new Success();
    }

    public async Task<OneOf<EmployeeForUpdateDto, NotFoundError>> GetForPatch(Guid companyId, Guid id)
    {
        var company = await _context.Companies.FindAsync(companyId);
        if (company is null)
        {
            _logger.LogWarning("A request to patch a company with a non exsistent id {CompanyId}", companyId);
            return new NotFoundError("There is no company with the provided id", companyId.ToString());
        }
        var employee = await _context.Employees.FindAsync(id);
        if (employee is null)
        {
            _logger.LogWarning("A request to update an employee with a non exsistent id {EmployeeId}", id);
            return new NotFoundError(message: "There is no employee with the provided Id", id: id.ToString());
        }

        var dto = new EmployeeForUpdateDto()
        {
            Name = employee.Name,
            Age = employee.Age,
            Position = employee.Position
        };

        return dto;
    }

    public async Task SaveChangesForPatch(EmployeeForUpdateDto dto, Guid id)
    {
        var entity = await _context.Employees.FindAsync(id);
        // ? How to handle nullablity.
        // * handling not found is already done in the GetForPatch method.
        entity!.Name = dto.Name;
        entity.Age = dto.Age;
        entity.Position = dto.Position;
        await _context.SaveChangesAsync();
    }
}
