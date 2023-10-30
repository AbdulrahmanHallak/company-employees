using System.Threading.RateLimiting;
using NLog;

namespace CompanyEmployees.Api.Configuration;
public static class RateLimiter
{
    public static IServiceCollection ConfigureRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(opts =>
        {
            opts.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>
            (
            httpcontext =>
            RateLimitPartition.GetFixedWindowLimiter(partitionKey: httpcontext.Connection.RemoteIpAddress?.ToString()
            ?? httpcontext.Request.Headers.Host.ToString()
            , factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                Window = TimeSpan.FromSeconds(30),
                PermitLimit = 10

            }));

            opts.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;

                var logger = LogManager.GetCurrentClassLogger();
                logger.Warn("To many requets from the user with ip {IpAddress} and host {Host}",
                context.HttpContext.Connection.RemoteIpAddress?.ToString(),
                context.HttpContext.Request.Host);

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    await context.HttpContext.Response.WriteAsync(
                        $"Too many requests. Please try again after {retryAfter.TotalMinutes} minute(s).", cancellationToken: token);
                }
                else
                {
                    await context.HttpContext.Response.WriteAsync(
                    "Too many requests. Please try again later. ", cancellationToken: token);
                }
            };
        });
        return services;
    }
}
