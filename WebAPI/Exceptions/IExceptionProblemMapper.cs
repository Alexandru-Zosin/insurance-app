using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebAPI.Exceptions;

public interface IExceptionProblemMapper
{
    bool CanHandle(Exception ex);
    int StatusCode { get; }
    bool IsClientDetailSafe { get; }
    bool ShouldLog { get; }
    LogLevel LogLevel { get; }

    ProblemDetails Map(HttpContext ctx, Exception ex, string traceId, bool includeClientDetails);
}
