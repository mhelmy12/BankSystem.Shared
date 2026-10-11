using System;


using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Domain;

namespace Shared.AspNetCore;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken ct)
    {
        var problemDetails = MapException(exception);

        LogException(exception, problemDetails.Status);

        httpContext.Response.StatusCode = problemDetails.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, ct);

        return true;
    }

    private ProblemDetails MapException(Exception exception) => exception switch
    {
        ValidationException ve => new ValidationProblemDetails(
            ve.Errors
              .GroupBy(e => e.PropertyName)
              .ToDictionary(
                  g => g.Key,
                  g => g.Select(e => e.ErrorMessage).ToArray()))
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed",
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
        },

        DomainException de => new ProblemDetails
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "Domain rule violated",
            Detail = de.Message,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.2"
        },

        UnauthorizedAccessException => new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.2"
        },

        _ => new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred",
            Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1"
        }
    };

    private void LogException(Exception exception, int? statusCode)
    {
        var level = statusCode switch
        {
            400 or 401 or 403 or 404 or 409 or 422 => LogLevel.Warning,
            _ => LogLevel.Error
        };

        _logger.Log(
            level,
            exception,
            "Request failed with {ExceptionType} ({StatusCode})",
            exception.GetType().Name,
            statusCode);
    }
}
