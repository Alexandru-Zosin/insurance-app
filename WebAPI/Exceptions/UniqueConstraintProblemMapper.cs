using System.Net;
using Application.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebAPI.Exceptions;

public sealed class UniqueConstraintProblemMapper : IExceptionProblemMapper
{
    public bool CanHandle(Exception ex) => ex is UniqueConstraintViolationException;

    public int StatusCode => (int)HttpStatusCode.Conflict;

    public bool IsClientDetailSafe => true;

    public bool ShouldLog => true;

    public LogLevel LogLevel => LogLevel.Warning;

    public ProblemDetails Map(HttpContext ctx, Exception ex, string traceId, bool includeClientDetails)
    {
        // Keep message generic unless you can safely provide a domain-specific one.
        var detail = includeClientDetails ? "Unique constraint violated." : null;

        var problem = new ProblemDetails
        {
            Status = StatusCode,
            Title = "Conflict.",
            Type = $"https://httpstatuses.com/{StatusCode}",
            Detail = detail,
            Instance = ctx.Request.Path
        };

        problem.Extensions["traceId"] = traceId;
        return problem;
    }
}
