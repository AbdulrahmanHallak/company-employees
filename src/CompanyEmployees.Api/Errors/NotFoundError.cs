using CompanyEmployees.Api.Errors;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Api.Errors;
public class NotFoundError : Error
{
    public string Id { get; private set; }
    public override string Title { get; protected set; } = "Not Found";
    public override int StatusCode { get; protected set; } = 404;

    public NotFoundError(string message, string id) : base(message)
    {
        Id = id;
    }

    public override ProblemDetails ToProblemDetails()
        => new ProblemDetails()
        {
            Title = Title,
            Status = StatusCode,
            Detail = Message,
            Instance = Id
        };
}
