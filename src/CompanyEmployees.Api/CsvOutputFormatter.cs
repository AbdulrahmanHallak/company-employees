using System.Text;
using CompanyEmployees.Api.Data.Entities;
using CompanyEmployees.Api.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace CompanyEmployees.Api;
public class CsvOutputFormatter : TextOutputFormatter
{
    public CsvOutputFormatter()
    {
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));

    }
    protected override bool CanWriteType(Type? type)
        => typeof(CompanyDto).IsAssignableFrom(type) || typeof(IEnumerable<CompanyDto>).IsAssignableFrom(type)
        || typeof(EmployeeDto).IsAssignableFrom(type) || typeof(IEnumerable<EmployeeDto>).IsAssignableFrom(type);

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<CsvOutputFormatter>>();
        var buffer = new StringBuilder();
        if (context.Object is CompanyDto company)
        {
            FormatCompanyCsv(buffer, company, logger);
        }
        else if (context.Object is IEnumerable<CompanyDto> companies)
        {
            foreach (var item in companies)
            {
                FormatCompanyCsv(buffer, item, logger);
            }
        }
        else if (context.Object is EmployeeDto employee)
        {
            FormatEmployeeCsv(buffer, employee, logger);
        }
        else if (context.Object is IEnumerable<EmployeeDto> employees)
        {
            foreach (var item in employees)
            {
                FormatEmployeeCsv(buffer, item, logger);
            }
        }
        await context.HttpContext.Response.WriteAsync(buffer.ToString());
    }

    private static void FormatCompanyCsv(StringBuilder buffer, CompanyDto company, ILogger<CsvOutputFormatter> logger)
    {
        logger.LogInformation("Writing company {Id} to the response", company.Id);
        buffer.AppendLine($"\"{company.Id}\",\"{company.Name}\",\"{company.FullAddress}\"");
    }
    private static void FormatEmployeeCsv(StringBuilder buffer, EmployeeDto employee, ILogger<CsvOutputFormatter> logger)
    {
        logger.LogInformation("Writing employee {Id} to the response", employee.Id);
        buffer.AppendLine($"\"{employee.Id}\",\"{employee.Name}\",\"{employee.Age}\",\"{employee.Position}\"");
    }
}
