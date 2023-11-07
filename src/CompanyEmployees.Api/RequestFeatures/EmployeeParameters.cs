using CompanyEmployees.Api.Models;
namespace CompanyEmployees.Api.RequestFeatures;

/// <summary>
/// Represents parameters for filtering and searching <see cref="EmployeeDto"/>.
/// </summary>
public class EmployeeParameters
{
    private int _minAge = 0;
    private int _maxAge = 100;

    /// <summary>
    /// Gets or sets the minimum age for filtering.
    /// </summary>
    /// <remarks>
    /// When provided with age greater than 100 and less than 0 it defaults to 0.
    /// </remarks>
    public int MinAge
    {
        get { return _minAge; }
        set
        {
            if (value < 0 || value > _maxAge) _minAge = 0;
            else _minAge = value;
        }
    }
    /// <summary>
    /// Gets or sets the maximum age for filtering.
    /// </summary>
    /// <remarks>
    /// When provided with age greater than 100 and less than 0 it defaults to 0.
    /// </remarks>
    public int MaxAge
    {
        get { return _maxAge; }
        set
        {
            if (value < 0 || value < _minAge) _maxAge = 100;
            else _maxAge = value;
        }
    }
    /// <summary>
    /// Gets or sets the search term for searching employees by name.
    /// </summary>
    public string? SearchTerm { get; set; }
}
