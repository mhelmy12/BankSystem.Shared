using System.Net;
using Microsoft.AspNetCore.Http;
using Shared.Helpers;

namespace Account_Service.Helpers;

public static class EndpointResponse
{
    public static IResult Result<T>(Response<T> response)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.NotFound:
                return Results.NotFound(response);
            case HttpStatusCode.BadRequest:
                return Results.BadRequest(response);
            case HttpStatusCode.InternalServerError:
                return Results.InternalServerError(response);
            case HttpStatusCode.Unauthorized:
                return Results.Unauthorized();
            case HttpStatusCode.UnprocessableEntity:
                return Results.UnprocessableEntity(response);
            case HttpStatusCode.Created:
                return Results.Created(response.Meta.ToString(), response);
            default:
                return Results.Ok(response);
        }

    }

}
