using System;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Shared.Helpers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An exception occurred: {Message}", exception.Message);

        int statusCode = StatusCodes.Status500InternalServerError;
        ProblemDetails problemDetails;
        var response = httpContext.Response;
        response.ContentType = "application/json";
        var responseModel = new Response<string>() { Succeeded = false, Message = exception?.Message ?? "" };
        switch (exception)
        {

            case ValidationException validationEx:
                statusCode = StatusCodes.Status422UnprocessableEntity;
                var errors = validationEx.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    );
                responseModel.Errors = errors;
                responseModel.StatusCode = System.Net.HttpStatusCode.UnprocessableEntity;
                responseModel.Succeeded = false;
                httpContext.Response.StatusCode = statusCode;
                await httpContext.Response.WriteAsJsonAsync(responseModel, cancellationToken);
                return true;

            case InvalidOperationException invalidOpEx:
                statusCode = StatusCodes.Status400BadRequest;
                problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = "Bad Request",
                    Detail = invalidOpEx.Message,
                    Instance = httpContext.Request.Path,

                };
                break;

            default:
                problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred in the system, please try again later.",
                    Instance = httpContext.Request.Path
                };
                break;
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

}
