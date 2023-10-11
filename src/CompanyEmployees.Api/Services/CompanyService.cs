using CompanyEmployees.Api.Data;
using CompanyEmployees.Api.Interfaces;
using CompanyEmployees.Api.Models;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IEnumerable<CompanyDto>> GetAllAsync(bool trackChanges, int count = 10)
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
}
