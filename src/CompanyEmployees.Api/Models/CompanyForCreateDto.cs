namespace CompanyEmployees.Api.Models;
public class CompanyForCreateDto
{
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string Country { get; set; } = default!;
}
