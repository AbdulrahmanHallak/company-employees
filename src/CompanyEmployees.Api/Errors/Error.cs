using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Api.Errors;

/// <summary>
/// Represents an abstract base class for defining custom error types in the application.
/// </summary>
/// <remarks>
/// When extending this class, it is essential to explicitly assign values to the <see cref="Title"/> and
/// <see cref="StatusCode"/> properties and not have them provided through the constrcutor at the time of initialzation.
/// This helps ensure consistency and accuracy of each specific error
/// information throughout the application.
/// </remarks>
public abstract class Error
{
    public abstract string Title { get; protected set; }
    public abstract int StatusCode { get; protected set; }
    public string Message { get; private set; }

    public Error(string message)
    {
        Message = message;
    }
    /// <summary>
    /// Converts the error into a <see cref="ProblemDetails"/> object for use in handling and reporting errors in api endpoints.
    /// </summary>
    /// <returns>A <see cref="ProblemDetails"/> object representing the error.</returns>
    public abstract ProblemDetails ToProblemDetails();

    public static implicit operator ProblemDetails(Error err) => err.ToProblemDetails();
}
