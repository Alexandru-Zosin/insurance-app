using Microsoft.AspNetCore.Mvc;
namespace WebAPI.Exceptions;

public interface IExceptionProblemMapper
{
    bool CanHandle(Exception ex);
    int StatusCode { get; }
    bool ShouldLog { get; }
    LogLevel LogLevel { get; }
    ProblemDetails Map(HttpContext ctx, Exception ex, string traceId);
}
