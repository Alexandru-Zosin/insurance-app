using System.Net;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Exceptions;

public sealed class DomainExceptionProblemMapper : IExceptionProblemMapper
{
    public bool CanHandle(Exception ex) => ex is DomainException;

    public int StatusCode => (int)HttpStatusCode.BadRequest;

    public bool IsClientDetailSafe => true;

    public bool ShouldLog => true;

    public LogLevel LogLevel => LogLevel.Warning;

    public ProblemDetails Map(HttpContext ctx, Exception ex, string traceId, bool includeClientDetails)
    {
        var de = (DomainException)ex;

        var problem = new ProblemDetails
        {
            Status = StatusCode,
            Title = "Domain rule violated.",
            Type = $"https://httpstatuses.com/{StatusCode}",
            Instance = ctx.Request.Path,
            Detail = includeClientDetails ? de.Message : null
        };

        problem.Extensions["traceId"] = traceId;
        return problem;
    }
}
