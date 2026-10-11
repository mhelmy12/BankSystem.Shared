using System;

namespace Shared.AspNetCore.Results;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;

public static class ResultExtensions
{
    public static IResult ToHttpResult(this Result result)
        => result.IsSuccess
            ? Results.NoContent()
            : MapError(result.Error);

    public static IResult ToHttpResult<T>(this Result<T> result)
        => result.IsSuccess
            ? Results.Ok(result.Value)
            : MapError(result.Error);

    public static IResult ToHttpResult<T>(this Result<T> result, Func<T, IResult> onSuccess)
        => result.IsSuccess
            ? onSuccess(result.Value)
            : MapError(result.Error);

    private static IResult MapError(Error error) => error.Type switch
    {
        ErrorType.Validation => Results.Problem(
            title: "Validation failed",
            detail: error.Message,
            statusCode: StatusCodes.Status400BadRequest),

        ErrorType.NotFound => Results.Problem(
            title: "Not found",
            detail: error.Message,
            statusCode: StatusCodes.Status404NotFound),

        ErrorType.Conflict => Results.Problem(
            title: "Conflict",
            detail: error.Message,
            statusCode: StatusCodes.Status409Conflict),

        ErrorType.Unauthorized => Results.Problem(
            title: "Unauthorized",
            detail: error.Message,
            statusCode: StatusCodes.Status401Unauthorized),

        ErrorType.Forbidden => Results.Problem(
            title: "Forbidden",
            detail: error.Message,
            statusCode: StatusCodes.Status403Forbidden),

        _ => Results.Problem(
            title: "An error occurred",
            detail: error.Message,
            statusCode: StatusCodes.Status500InternalServerError)
    };
}