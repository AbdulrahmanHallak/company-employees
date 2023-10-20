namespace CompanyEmployees.Api.Models;
public class EmployeeForUpdateDto
{
    public string Name { get; set; } = default!;
    public int Age { get; set; }
    public string Position { get; set; } = default!;
}
