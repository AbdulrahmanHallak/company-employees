namespace CompanyEmployees.Api.Models;
public class TokenDto
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}
