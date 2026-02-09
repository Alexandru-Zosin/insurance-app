using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebAPI.Exceptions;

public sealed class FluentValidationProblemMapper : IExceptionProblemMapper
{
    public bool CanHandle(Exception ex) => ex is ValidationException;

    public int StatusCode => (int)HttpStatusCode.BadRequest;

    public bool IsClientDetailSafe => true;

    public bool ShouldLog => false; // usually not worth logging

    public LogLevel LogLevel => LogLevel.Warning;

    public ProblemDetails Map(HttpContext ctx, Exception ex, string traceId, bool includeClientDetails)
    {
        var ve = (ValidationException)ex;

        var errors = ve.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).Distinct().ToArray());

        var vpd = new ValidationProblemDetails(errors)
        {
            Status = StatusCode,
            Title = "Validation failed.",
            Type = $"https://httpstatuses.com/{StatusCode}",
            Instance = ctx.Request.Path
        };

        vpd.Extensions["traceId"] = traceId;
        return vpd;
    }
}
