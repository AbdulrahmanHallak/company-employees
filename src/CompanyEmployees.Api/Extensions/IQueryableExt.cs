using CompanyEmployees.Api.Data.Entities;
using System.Reflection;
using System.Text;
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

        var orderParam = orderByQueryString.Trim().Split(',');
        var propertyInfos = typeof(Employee).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var orderQueryBuilder = new StringBuilder();

        foreach (var param in orderParam)
        {
            if (string.IsNullOrWhiteSpace(param))
                continue;

            var propertyFromQueryName = param.Split(" ")[0];

            var objectProperty = propertyInfos
                .FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

            if (objectProperty is null)
                continue;

            var direction = param.EndsWith(" desc") ? "descending" : "ascending";

            orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {direction},");
        }

        var orderQuery = orderQueryBuilder.ToString().Trim(',', ' ');

        if (string.IsNullOrWhiteSpace(orderQuery))
            return employees.OrderBy(x => x.Name);

        return employees.OrderBy(orderQuery);
    }
}
