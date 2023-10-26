using CompanyEmployees.Api.Data.Entities;
using System.Linq.Dynamic.Core;

namespace CompanyEmployees.Api.Extensions;
public static class IQueryableExt
{
    public static IQueryable<Employee> SearchByName(this IQueryable<Employee> employees, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return employees;

        return employees.Where(x => x.Name.ToLower().Contains(searchTerm.Trim().ToLower()));
    }

    public static IQueryable<Employee> FilterByAge(this IQueryable<Employee> employees, int minAge, int maxAge)
            => employees.Where(x => x.Age >= minAge && x.Age <= maxAge);

    public static IQueryable<Employee> Sort(this IQueryable<Employee> employees, string? orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return employees.OrderBy(x => x.Name);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Employee>(orderByQueryString);

        if (string.IsNullOrWhiteSpace(orderQuery))
            return employees.OrderBy(x => x.Name);

        return employees.OrderBy(orderQuery);
    }
}
