namespace CompanyEmployees.Api.Models;
public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string FullAddress { get; set; } = default!; // Combine the Address and Company properties on the Entity.

}
