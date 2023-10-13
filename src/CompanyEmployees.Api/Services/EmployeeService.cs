using CompanyEmployees.Api.Data;
using CompanyEmployees.Api.Interfaces;
using CompanyEmployees.Api.Models;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace CompanyEmployees.Api.Services;
public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;

    public EmployeeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OneOf<IEnumerable<EmployeeDto>, NotFound>> GetAsync(bool trackChanges, Guid companyId, int count = 10)
    {
        // Check if the company exists.
        var company = await _context.Companies.FindAsync(companyId);
        if (company is null) return new NotFound();

        // The actual query.
        IEnumerable<EmployeeDto> employees;

        if (trackChanges)
            employees = await
                (from emp in _context.Employees
                 where emp.CompanyId == companyId
                 select new EmployeeDto()
                 {
                     Id = emp.Id,
                     Name = emp.Name,
                     Position = emp.Position,
                     Age = emp.Age
                 }).Take(count).ToListAsync();

        else
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
}
