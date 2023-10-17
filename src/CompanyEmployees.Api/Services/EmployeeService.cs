using CompanyEmployees.Api.Data;
using CompanyEmployees.Api.Data.Entities;
using CompanyEmployees.Api.Errors;
using CompanyEmployees.Api.Interfaces;
using CompanyEmployees.Api.Models;
using Microsoft.EntityFrameworkCore;
using OneOf;

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

    public async Task<OneOf<IEnumerable<EmployeeDto>, NotFoundError>> GetAsync(Guid companyId, int count = 10)
    {
        // Check if the company exists.
        var company = await _context.Companies.FindAsync(companyId);
        if (company is null)
        {
            _logger.LogWarning("A request to retrieve employees of a company with a non-exsistent id {CompanyId}", companyId);
            return new NotFoundError(message: "There is no company with the provided id", id: companyId.ToString());
        }

        // The actual query.
        IEnumerable<EmployeeDto> employees;
        employees = await
            (from emp in _context.Employees.AsNoTracking()
             where emp.CompanyId == companyId
             select new EmployeeDto()
             {
                 Id = emp.Id,
                 Name = emp.Name,
                 Position = emp.Position,
                 Age = emp.Age
             }).Take(count).ToListAsync();

        return (List<EmployeeDto>)employees;
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
}
