namespace CompanyEmployees.Api.Data.Models;
public class Company
{
    // All fields are required and they are set to default to suppress null warnings.
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string Country { get; set; } = default!;
    public ICollection<Employee> Employees { get; set; } = default!;
}
