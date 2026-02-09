using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace WebAPI.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IReadOnlyList<IExceptionProblemMapper> _mappers;
    private readonly bool _includeClientDetails;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IConfiguration configuration,
        IEnumerable<IExceptionProblemMapper> mappers)
    {
        _logger = logger;
        _mappers = mappers.ToList();
        _includeClientDetails = configuration.GetValue<bool>("Errors:IncludeClientDetails");
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        var mapper = _mappers.FirstOrDefault(m => m.CanHandle(exception));
        if (mapper is null)
        {
            // If you registered a fallback mapper, this should never happen.
            _logger.LogError(exception, "No exception mapper registered. TraceId: {TraceId}", traceId);
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(new
            {
                type = "https://httpstatuses.com/500",
                title = "An unexpected error occurred.",
                status = 500,
                traceId
            }, cancellationToken);
            return true;
        }

        var includeClientDetails = _includeClientDetails && mapper.IsClientDetailSafe;
        var problem = mapper.Map(httpContext, exception, traceId, includeClientDetails);

        if (mapper.ShouldLog)
        {
            _logger.Log(
                mapper.LogLevel,
                exception,
                "Request failed with {StatusCode}. TraceId: {TraceId}",
                mapper.StatusCode,
                traceId);
        }

        httpContext.Response.StatusCode = mapper.StatusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
