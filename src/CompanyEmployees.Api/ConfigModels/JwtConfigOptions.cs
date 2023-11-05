namespace CompanyEmployees.Api.ConfigModels;
public class JwtSettings
{
    public string ValidIssuer { get; set; } = default!;
    public string ValidAudience { get; set; } = default!;
    public int Expires { get; set; }
}
