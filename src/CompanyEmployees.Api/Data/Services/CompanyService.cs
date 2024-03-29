using CompanyEmployees.Api.Data.Entities;
using CompanyEmployees.Api.Errors;
using CompanyEmployees.Api.Extensions;
using CompanyEmployees.Api.Interfaces;
using CompanyEmployees.Api.Models;
using CompanyEmployees.Api.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace CompanyEmployees.Api.Data.Services;
public class CompanyService : ICompanyService
{
    private readonly AppDbContext _context;
    private readonly ILogger<CompanyService> _logger;

    public CompanyService(AppDbContext context, ILogger<CompanyService> logger)
    {
        _context = context;
        _logger = logger;
    }
    public Task<PaginatedList<CompanyDto>> GetAsync(PaginationFilter? filter)
    {
        if (filter is null) filter = new PaginationFilter();

        IQueryable<CompanyDto> companies =
            _context.Companies
            .Sort(filter.OrderBy)
            .Select(x => new CompanyDto()
            {
                Id = x.Id,
                Name = x.Name,
                FullAddress = string.Join(' ', x.Address, x.Country)
            });

        return PaginatedList<CompanyDto>.CreateAsync(companies, filter.PageNumber, filter.PageSize);
    }
    public async Task<OneOf<CompanyDto, NotFoundError>> GetAsync(Guid id)
    {
        CompanyDto? company;
        company = await
            (from comp in _context.Companies
             where comp.Id == id
             select new CompanyDto()
             {
                 Id = comp.Id,
                 Name = comp.Name,
                 FullAddress = string.Join(' ', comp.Address, comp.Country)
             }).SingleOrDefaultAsync();

        if (company is null)
        {
            _logger.LogWarning("A request to retrieve a company with a non exsistent id {CompanyId}", id);
            return new NotFoundError("There is no company with the provided id", id.ToString());
        }
        else return company;
    }

    public async Task<OneOf<CompanyDto, InternalServerError>> CreateAsync(CompanyForCreateDto dto)
    {
        // Map to entity.
        var company = new Company()
        {
            Name = dto.Name,
            Address = dto.Address,
            Country = dto.Country,
            // The user has the option to include a list of employees in the same request for creating a company,
            // thus eliminating the need to make a separate request for creating employees associated with the newly created company.
            Employees = dto.Employees?.Select(x => new Employee() { Age = x.Age, Name = x.Name, Position = x.Position }).ToList()!
        };

        _context.Companies.Add(company);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("the following exception was thrown when saving {@Entity}: {Exception}", company, ex);
            return new InternalServerError("An error occurred while processing the request. Please contact support");
        }
        return new CompanyDto()
        {
            Id = company.Id,
            Name = company.Name,
            FullAddress = string.Join(' ', company.Address, company.Country)
        };
    }

    public async Task DeleteAsync(Guid id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company is null)
        {
            _logger.LogWarning("Request to delete a non-existent company with the following id: {CompnayId}", id);
            return;
        }
        _context.Remove(company);
        await _context.SaveChangesAsync();
    }

    public async Task<OneOf<Success, NotFoundError>> UpdateAsync(Guid id, CompanyForUpdateDto dto)
    {
        // Check if company exists:
        var company = await _context.Companies.FindAsync(id);
        if (company is null)
        {
            _logger.LogWarning("A request to update a company with a non exsistent id {CompanyId}", id);
            return new NotFoundError("There is no company with the provided id", id.ToString());
        }
        // map to entity:
        company.Address = dto.Address;
        company.Name = dto.Name;
        company.Country = dto.Country;
        company.Employees = dto.Employees?.Select(x => new Employee() { Age = x.Age, Name = x.Name, Position = x.Position }).ToList()!;

        await _context.SaveChangesAsync();
        return new Success();
    }
}
