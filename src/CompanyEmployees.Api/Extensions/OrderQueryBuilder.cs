using System.Reflection;
using System.Text;

namespace CompanyEmployees.Api.Extensions;
public static class OrderQueryBuilder
{
    public static string CreateOrderQuery<T>(string orderByQueryString)
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
