using System.Net;
using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Exceptions;

public sealed class DuplicateKeyExceptionMapper : IExceptionProblemMapper
{
    public bool CanHandle(Exception ex) => ex is DuplicateKeyException;
    public int StatusCode => (int)HttpStatusCode.Conflict;
    public bool ShouldLog => true;
    public LogLevel LogLevel => LogLevel.Warning;

    public ProblemDetails Map(HttpContext ctx, Exception ex, string traceId)
    {
        var problem = new ProblemDetails
        {
            Status = StatusCode,
            Title = "Conflict.",
            Type = $"https://httpstatuses.com/{StatusCode}",
            Detail = "A resource with the same unique value already exists.",
            Instance = ctx.Request.Path
        };

        problem.Extensions["code"] = "duplicate_key";
        problem.Extensions["traceId"] = traceId;

        return problem;
    }
}
