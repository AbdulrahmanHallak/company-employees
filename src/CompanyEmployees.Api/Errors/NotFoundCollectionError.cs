using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Api.Errors;
public sealed class NotFoundCollectionError : Error
{

    public override string Title { get; protected set; } = "Not Found";
    public override int StatusCode { get; protected set; } = 404;
    public IDictionary<string, List<Guid>> NotFoundCollection { get; private set; }
    public NotFoundCollectionError(string message, IDictionary<string, List<Guid>> notFoundCollection) : base(message)
    {
        NotFoundCollection = notFoundCollection;
    }

    public override ProblemDetails ToProblemDetails()
    {
        var prop = new ProblemDetails()
        {
            Title = Title,
            Detail = Message,
            Status = StatusCode,
        };
        foreach (var item in NotFoundCollection)
            prop.Extensions.Add(item.Key, item.Value);

        return prop;
    }
}
