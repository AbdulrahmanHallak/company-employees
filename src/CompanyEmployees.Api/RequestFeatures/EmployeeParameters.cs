namespace CompanyEmployees.Api.RequestFeatures;
public class EmployeeParameters
{
    private int _minAge = 0;
    private int _maxAge = 100;
    public int MinAge
    {
        get { return _minAge; }
        set
        {
            if (value < 0 || value > _maxAge) _minAge = 0;
            else _minAge = value;
        }
    }
    public int MaxAge
    {
        get { return _maxAge; }
        set
        {
            if (value < 0 || value < _minAge) _maxAge = 100;
            else _maxAge = value;
        }
    }

    public string? SearchTerm { get; set; }
}
