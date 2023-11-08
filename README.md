# Company Employees

A robust and secure REST API solution designed to efficiently handle the management of companies and their associated employees. This API offers a comprehensive set of functionalities and features, catering to CRUD operations (Create, Read, Update, Delete) through various HTTP methods, including GET, PUT, POST, DELETE, and PATCH.

## Key Features
1. [Authentication using ASP.NET Core Identity](#Authentication-and-Authorization)
2. [Authorization with access tokens and refresh tokens](#Authentication-and-Authorization)
3. [Pagination, Sorting, Filtering, and Searching](#pagination-sorting-filtering-and-searching)
4. [Validation with FlunetValidation library](#validation-with-fluentvalidation)
5. [Versioning](#versioning)
6. [Rate Limiting](#rate-limiting)
7. [Explicit error handling with custom error types and OneOf library](#explicit-error-handling-with-custom-error-types-and-oneof-library)
8. [Swagger documentation](#swagger-documentation)
9. [Custom ISchemaFilter for FluentValidation integration with Swagger](#custom-ischemafilter-for-fluentvalidation-integration-with-swagger)
10. Custom CSV output formatter
11. Nlog for logging

### Authentication and Authorization
The authentication system is built upon ASP.NET Core Identity, ensuring secure access to the API. The implementation utilizes Role-Based Authorization, defining two primary roles: "Manager" and "Administrator." The "Manager" role is granted read-only access to all resources, while the "Administrator" role possesses read-write access across the entire application.

The authentication process includes specific endpoints for various user actions, such as registering users, signing in, and refreshing access tokens.

![Authentication Controller](/images/AuthController.png)

The token creation, user registration, login, and token refresh functionalities are encapsulated within a dedicated service located in the Data/Services directory. This service efficiently manages the  interaction with database and the process of generating tokens, registering users, and handling login and token refresh procedures.


### Pagination, Sorting, Filtering and Searching

The implementation incorporates robust functionalities for Pagination, Sorting, Filtering, and Searching within the API.

Pagination is achieved through a Paginated list wrapper type:

![PaginatedList](/images/PaginatedList.png)

this ensures that pagination operations are encapsulated within a strongly-typed object. The pagination system, encapsulated within a Paginated list wrapper type, ensures an efficient and explicit handling of pagination. Its strongly-typed nature allows for safe usage across various layers of the application. Furthermore, this approach not only facilitates backend operations but also significantly eases frontend development by providing a structured and clear approach to pagination implementation.

To enable sorting, a generic extension method has been introduced. This method dynamically constructs a string that serves as a parameter for the OrderBy method with the capabilities of the [System.Linq.Dynamic.Core](https://dynamic-linq.net/) library. This solution allows for the creation of sorting rules in a string format, offering flexibility and adaptability in sorting data according to various parameters and types.

![OrderQueryBuilder method](/images/OrderQueryBuilder.png)


### Validation with FlunetValidation

The API implements validation using the FluentValidation library, which offers a more comprehensive and flexible approach to validating DTOs. This choice diverges from using DataAnnotations attributes, primarily due to the limitations of the latter in handling complex validation scenarios.

Validators, responsible for validating DTOs, are located within the Validators directory

### Versioning

The project includes a minimal implementation of versioning to showcase an understanding of this feature. Versioning has been applied within the API, despite the project's relatively small scale, to demonstrate proficiency in handling API version control.

![Versioning](/images/Versioning.png)

While the implementation is minimal in this context, it serves as a demonstration of knowledge and skills in setting up and managing API versioning.


### Rate Limiting

The API incorporates rate limiting using the most recent rate limiter middleware introduced in ASP.NET Core 7. Despite having fewer features compared to the [AspNetCoreRateLimit](https://www.nuget.org/packages/AspNetCoreRateLimit) library, this native middleware effectively serves the project's needs, especially considering the project's relatively smaller scale.

![RateLimiter Class](/images/RateLimiter.png)

This middleware was chosen due to its suitability for the current project size and requirements. Additionally, there's an ongoing open [issue](https://github.com/dotnet/aspnetcore/issues/44140) on GitHub aiming to enhance the functionality of this middleware, which assures that future updates might address the limitations, making it a pragmatic choice without introducing unnecessary overhead, which aligns well with the project's size and objectives.


### Explicit error handling with custom error types and OneOf library

The API emphasizes explicit error handling by implementing custom error types and leveraging the [OneOf](https://github.com/mcintyre321/OneOf) library. This deliberate choice moves away from traditional exception-based error handling due to its inherent complexities and replaces it with a more structured and explicit approach. The custom error types, designed with an implicit casting operator to ProblemDetails, play a crucial role in the API's error handling strategy. These custom error types, with their seamless conversion to ProblemDetails, enhance consistency and facilitate a standardized error format throughout the API.

By employing ProblemDetails as the primary error type returned from the API, this common error format ensures a unified structure for conveying errors. The implicit casting operator integrated into the custom error types simplifies the process of converting these errors to the ProblemDetails format, thereby promoting consistency and adherence to a standardized error output across the API endpoints.

![Error Class](/images/ErroClass.png)

An implemenation example:

![NotFoundError Class](/images/NotFoundErrorClass.png)

Also, by utilizing the OneOf library, which provides discriminated unions similar to those found in F#, the API defines clear, specific error types. These custom error types allow for method signatures to explicitly showcase the possible return types, offering enhanced clarity in the expected outputs of these methods. This not only refines the codebase but also aids developers in understanding the potential outcomes and handling different scenarios with greater precision.

### Swagger Documentation
The API is equipped with comprehensive Swagger documentation that covers all controllers and endpoints:

![Swagger docs](/images/SwaggerDocs.png)


### Custom ISchemaFilter for FluentValidation integration with Swagger

The API utilizes a custom ISchemaFilter to seamlessly integrate FluentValidation rules with Swagger documentation. While Swagger inherently integrates with DataAnnotation Attributes, the project employs the FluentValidation library for validation instead, requiring a custom solution to incorporate these validation rules into the Swagger documentation.

This custom ISchemaFilter serves as a bridge between the FluentValidation rules and Swagger, allowing the API to auto-generate documentation for validation rules defined using the FluentValidation library.

![ISchemaFilter implementation](/images/SwaggerFluentValidation.png)