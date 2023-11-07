using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.Api.RequestFeatures;
/// <summary>
/// Represents a paginated list of items of type T.
/// </summary>
/// <typeparam name="T">The type of items in the paginated list.</typeparam>
public class PaginatedList<T>
{
    /// <summary>
    /// Gets the current page index.
    /// </summary>
    public int PageIndex { get; private set; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages { get; private set; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    public int PageSize { get; private set; }

    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    /// <summary>
    /// Gets the collection of items in the current page.
    /// </summary>
    public IEnumerable<T> Data { get; private set; }

    /// <summary>
    /// Initializes a new instance of the PaginatedList class.
    /// </summary>
    /// <param name="items">The list of items to be paginated.</param>
    /// <param name="count">The total count of items.</param>
    /// <param name="pageIndex">The index of the current page.</param>
    /// <param name="pageSize">The number of items per page.</param>
    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageSize = pageSize;
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        Data = items;
    }

    /// <summary>
    /// Creates a paginated list asynchronously based on the source IQueryable.
    /// </summary>
    /// <param name="source">The source IQueryable collection.</param>
    /// <param name="pageIndex">The index of the current page.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A Task representing the asynchronous creation of the PaginatedList.</returns>
    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip(
            (pageIndex - 1) * pageSize)
            .Take(pageSize).ToListAsync();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}