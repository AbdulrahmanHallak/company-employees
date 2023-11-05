using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Api.Errors;
public class InvalidTokenError : Error
{
    public override string Title { get; protected set; } = "InvalidToken";
    public override int StatusCode { get; protected set; } = 400;
    public InvalidTokenError(string message) : base(message) { }
    public override ProblemDetails ToProblemDetails()
        => new ProblemDetails() { Detail = Message, Status = StatusCode, Title = Title };
}
