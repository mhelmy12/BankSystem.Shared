using System;


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;

namespace Shared.AspNetCore.Results;

public static class ResultExtensions
{
    public static IResult ToHttpResult(this Result result)
        => result.IsSuccess
            ? Microsoft.AspNetCore.Http.Results.NoContent()
            : MapError(result.Error);

    public static IResult ToHttpResult<T>(this Result<T> result)
        => result.IsSuccess
            ? Microsoft.AspNetCore.Http.Results.Ok(result.Value)
            : MapError(result.Error);

    public static IResult ToHttpResult<T>(this Result<T> result, Func<T, IResult> onSuccess)
        => result.IsSuccess
            ? onSuccess(result.Value)
            : MapError(result.Error);

    private static IResult MapError(Error error) => error.Type switch
    {
        ErrorType.Validation => Microsoft.AspNetCore.Http.Results.Problem(
            title: "Validation failed",
            detail: error.Message,
            statusCode: StatusCodes.Status400BadRequest),

        ErrorType.NotFound => Microsoft.AspNetCore.Http.Results.Problem(
            title: "Not found",
            detail: error.Message,
            statusCode: StatusCodes.Status404NotFound),

        ErrorType.Conflict => Microsoft.AspNetCore.Http.Results.Problem(
            title: "Conflict",
            detail: error.Message,
            statusCode: StatusCodes.Status409Conflict),

        ErrorType.Unauthorized => Microsoft.AspNetCore.Http.Results.Problem(
            title: "Unauthorized",
            detail: error.Message,
            statusCode: StatusCodes.Status401Unauthorized),

        ErrorType.Forbidden => Microsoft.AspNetCore.Http.Results.Problem(
            title: "Forbidden",
            detail: error.Message,
            statusCode: StatusCodes.Status403Forbidden),

        _ => Microsoft.AspNetCore.Http.Results.Problem(
            title: "An error occurred",
            detail: error.Message,
            statusCode: StatusCodes.Status500InternalServerError)
    };
}