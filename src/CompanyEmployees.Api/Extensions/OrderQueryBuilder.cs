using System.Reflection;
using System.Text;

namespace CompanyEmployees.Api.Extensions;
public static class OrderQueryBuilder
{
    /// <summary>
    /// Creates an order query string based on the provided orderByQueryString for a specified type.
    /// </summary>
    /// <typeparam name="T">The type for which the order query string is being created.</typeparam>
    /// <param name="orderByQueryString">The string containing order parameters.</param>
    /// <returns>
    /// A string representing the order query for the specified type based on the provided parameters.
    /// </returns>
    /// <remarks>
    /// The returned string is only useful to use with System.Linq.Dynamic.Core library,
    /// and it could be empty if the properties provided in the string
    /// parameter do not exist in the specified type.
    ///     Sample:
    ///             orderByQueryString = "name desc,age"
    ///             the method will return "name descending, age ascending"
    /// </remarks>
    public static string? CreateOrderQuery<T>(string orderByQueryString)
    {
        var orderParam = orderByQueryString.Trim().Split(',');
        var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

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

            orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {direction}, ");
        }

        var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
        return orderQuery;
    }
}
