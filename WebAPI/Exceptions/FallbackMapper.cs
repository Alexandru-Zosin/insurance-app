using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Exceptions;

public sealed class FallbackProblemMapper : IExceptionProblemMapper
{
    public bool CanHandle(Exception ex) => true;
    public int StatusCode => (int)HttpStatusCode.InternalServerError;
    public LogLevel LogLevel => LogLevel.Error;
    public bool ShouldLog => true;

    public ProblemDetails Map(HttpContext ctx, Exception ex, string traceId)
    {
        var problem = new ProblemDetails
        {
            Status = StatusCode,
            Title = "An unexpected error occurred.",
            Type = $"https://httpstatuses.com/{StatusCode}",
            Instance = ctx.Request.Path,
            Detail = null
        };

        problem.Extensions["traceId"] = traceId;
        return problem;
    }
}
