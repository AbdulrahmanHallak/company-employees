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
    public async Task<IEnumerable<CompanyDto>> GetAsync(bool trackChanges, int count = 10)
    {
        try
        {
            if (trackChanges)
                return await _context.Companies.Select(x => new CompanyDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    FullAddress = string.Join(' ', x.Address, x.Country)
                }).Take(count).ToListAsync();

            else
                return await _context.Companies.Select(x => new CompanyDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    FullAddress = string.Join(' ', x.Address, x.Country)
                }).Take(count).AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occured while retrieving entities from db {Exception}", ex);
            throw;
        }
    }
    public async Task<OneOf<CompanyDto, NotFound>> GetAsync(bool trackChanges, Guid id)
    {
        CompanyDto? company;
        if (trackChanges)
            company = await
                (from comp in _context.Companies
                 where comp.Id == id
                 select new CompanyDto()
                 {
                     Id = comp.Id,
                     Name = comp.Name,
                     FullAddress = $"{comp.Address} {comp.Country}"
                 }).SingleOrDefaultAsync();

        else
            company = await
                (from comp in _context.Companies.AsNoTracking()
                 where comp.Id == id
                 select new CompanyDto()
                 {
                     Id = comp.Id,
                     Name = comp.Name,
                     FullAddress = $"{comp.Address} {comp.Country}"
                 }).SingleOrDefaultAsync();

        if (company is null)
        {
            _logger.LogWarning("A request to retrieve a company with a non exsistent id {Id}", id);
            return new NotFound();
        }
        else return company;
    }
}
