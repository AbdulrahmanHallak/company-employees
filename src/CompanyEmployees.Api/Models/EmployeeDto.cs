namespace CompanyEmployees.Api.Models;
public class EmployeeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public int Age { get; set; }
    public string Position { get; set; } = default!;
}
