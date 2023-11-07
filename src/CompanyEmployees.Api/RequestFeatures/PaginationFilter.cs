namespace CompanyEmployees.Api.RequestFeatures;

/// <summary>
/// Represents a pagination configuration.
/// </summary>
public class PaginationFilter
{
    const int maxPageSize = 50;
    private int _pageSize = 10;

    /// <summary>
    /// Gets or sets the page number.
    /// </summary>
    public int PageNumber { get; set; } = 1;
    /// <summary>
    /// Gets or sets the page size. Maximum page size is 50.
    /// </summary>
    /// <remarks>
    /// Defaults to 10. Cannot be greater than 50.
    /// </remarks>
    public int PageSize
    {
        get { return _pageSize; }
        set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
    }
    /// <summary>
    /// Gets or sets the sorting criteria.
    /// </summary>
    /// <remarks>
    /// Sorting is included in this type because pagination does not
    /// work without sorting.
    /// </remarks>
    public string? OrderBy { get; set; }
}