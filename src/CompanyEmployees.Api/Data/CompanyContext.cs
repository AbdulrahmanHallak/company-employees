using CompanyEmployees.Api.Data.Models;
using CompanyEmployees.Api.Data.SeedData;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.Api.Data;
public class AppDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Company> Companies { get; set; }

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // All non-nullable reference typs are required by default in a NRT-enabled context,
        // which spare the need to call IsRequired.

        // Configure the Company entity.
        modelBuilder.Entity<Company>()
            .Property(x => x.Name)
            .HasMaxLength(60);

        modelBuilder.Entity<Company>()
            .Property(x => x.Address)
            .HasMaxLength(60);

        modelBuilder.Entity<Company>()
            .Property(x => x.Country)
            .HasMaxLength(40);

        // Configure the Employee entity.
        modelBuilder.Entity<Employee>()
            .Property(x => x.Name)
            .HasMaxLength(30);

        modelBuilder.Entity<Employee>()
            .Property(x => x.Position)
            .HasMaxLength(20);

        // Adding seed data
        modelBuilder.ApplyConfiguration(new CompanySeed());
        modelBuilder.ApplyConfiguration(new EmployeeSeed());
    }
}