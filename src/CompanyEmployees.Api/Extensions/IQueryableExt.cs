using CompanyEmployees.Api.Data.Entities;

namespace CompanyEmployees.Api.Extensions;
public static class IQueryableExt
{
    public static IQueryable<Employee> SearchByName(this IQueryable<Employee> employees, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return employees;

        return employees.Where(x => x.Name.ToLower().Contains(searchTerm.Trim().ToLower()));
    }

    public static IQueryable<Employee> FilterByAge(this IQueryable<Employee> employees, int minAge, int maxAge)
            => employees.Where(x => x.Age >= minAge && x.Age <= maxAge);
}
