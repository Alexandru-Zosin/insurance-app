using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;

namespace WebAPI.Exceptions;

public sealed class GlobalExceptionHandler(
     ILogger<GlobalExceptionHandler> logger,
     IEnumerable<IExceptionProblemMapper> problemMappers
    ) : IExceptionHandler
{
    private readonly IReadOnlyList<IExceptionProblemMapper> _problemMappers = problemMappers.ToList();

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var requestTraceId = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;

        var selectedMapper = _problemMappers.FirstOrDefault(m => m.CanHandle(exception));
        if (selectedMapper!.ShouldLog)
        {
            logger.Log(
                selectedMapper.LogLevel,
                exception,
                "Request failed with {StatusCode}. TraceId: {TraceId}",
                selectedMapper.StatusCode,
                requestTraceId);
        }

        var problemDetails = selectedMapper.Map(httpContext, exception, requestTraceId);
        httpContext.Response.StatusCode = selectedMapper.StatusCode;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}
