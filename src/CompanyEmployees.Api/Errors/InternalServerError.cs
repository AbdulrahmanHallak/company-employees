using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Api.Errors;
public class InternalServerError : Error
{
    public override string Title { get; protected set; } = "Internal server error";
    public override int StatusCode { get; protected set; } = 500;

    public InternalServerError(string message) : base(message)
    {
    }


    public override ProblemDetails ToProblemDetails()
        => new ProblemDetails()
        {
            Title = Title,
            Status = StatusCode,
            Detail = Message,
        };
}
