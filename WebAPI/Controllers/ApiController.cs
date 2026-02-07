using Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
public abstract class ApiController : ControllerBase
{
    protected ActionResult MapError(ErrorType type, string? message) =>
        type switch
        {
            ErrorType.NotFound => NotFound(message),
            ErrorType.Conflict => Conflict(message),
            ErrorType.Validation => BadRequest(message),
            ErrorType.Denied => Forbid(),
            _ => StatusCode(500, message)
        };

    protected ActionResult FromResult(Result result) =>
        result.IsSuccess
            ? NoContent()
            : MapError(result.ErrorType, result.ErrorMessage);

    protected ActionResult<T> FromResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            if (result.Value is null)
                return NoContent();

            return Ok(result.Value);
        }

        return MapError(result.ErrorType, result.ErrorMessage);
    }

    protected ActionResult<T> FromCreated<T>(
        Result<T> result,
        string location)
    {
       if (result.IsSuccess)
            return Created(location, result.Value);

        return MapError(result.ErrorType, result.ErrorMessage);
    }
}
