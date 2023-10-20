namespace CompanyEmployees.Api.Models;
public class CompanyForUpdateDto
{
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string Country { get; set; } = default!;

    // The user has the option to include a list of employees in the same request for updating a company,
    // thus eliminating the need to make a separate request for inserting employees associated with the updated company.
    public IEnumerable<EmployeeForCreateDto>? Employees { get; set; }
}
