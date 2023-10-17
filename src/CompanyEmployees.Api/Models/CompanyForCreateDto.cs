namespace CompanyEmployees.Api.Models;
public class CompanyForCreateDto
{
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string Country { get; set; } = default!;

    // The user has the option to include a list of employees in the same request for creating a company,
    // thus eliminating the need to make a separate request for creating employees associated with the newly created company.
    public ICollection<EmployeeForCreateDto>? Employees { get; set; }
}
