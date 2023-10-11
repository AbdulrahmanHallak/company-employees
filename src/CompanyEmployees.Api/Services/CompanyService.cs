using CompanyEmployees.Api.Data;
using CompanyEmployees.Api.Data.Models;
using CompanyEmployees.Api.Interfaces;
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
    public async Task<IEnumerable<Company>> GetAllAsync(bool trackChanges, int count = 10)
    {
        try
        {
            if (trackChanges)
                return await _context.Companies.Take(count).ToListAsync();

            return await _context.Companies.Take(count).AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occured while retrieving entities from db {Exception}", ex);
            throw;
        }
    }
}
