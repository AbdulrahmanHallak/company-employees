using CompanyEmployees.Api.Data;
using CompanyEmployees.Api.Interfaces;
using CompanyEmployees.Api.Models;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace CompanyEmployees.Api.Services;
public class CompanyService : ICompanyService
{
    private readonly AppDbContext _context;
    private readonly ILogger<CompanyService> _logger;

    public CompanyService(AppDbContext context, ILogger<CompanyService> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<IEnumerable<CompanyDto>> GetAsync(int count = 10)
    {
        return await _context.Companies.AsNoTracking().Select(x => new CompanyDto()
        {
            Id = x.Id,
            Name = x.Name,
            FullAddress = string.Join(' ', x.Address, x.Country)
        }).Take(count).ToListAsync();
    }
    public async Task<OneOf<CompanyDto, NotFound>> GetAsync(Guid id)
    {
        CompanyDto? company;
        company = await
            (from comp in _context.Companies.AsNoTracking()
             where comp.Id == id
             select new CompanyDto()
             {
                 Id = comp.Id,
                 Name = comp.Name,
                 FullAddress = string.Join(' ', comp.Address, comp.Country)
             }).SingleOrDefaultAsync();

        if (company is null)
        {
            _logger.LogWarning("A request to retrieve a company with a non exsistent id {Id}", id);
            return new NotFound();
        }
        else return company;
    }
}
