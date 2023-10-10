namespace CompanyEmployees.Api.Data.Models;
public class Employee
{
    // All fields are required and they are set to default to suppress null warnings.
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public int Age { get; set; }
    public string Position { get; set; } = default!;
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = default!;
}
