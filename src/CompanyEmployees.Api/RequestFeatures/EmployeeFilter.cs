namespace CompanyEmployees.Api.RequestFeatures;
public class EmployeeFilter
{
    public int MinAge { get; set; } = 0;
    public int MaxAge { get; set; } = 100;

    public bool ValidAgeRange => MinAge < MaxAge;
}
