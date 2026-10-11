using System.Net;
using Microsoft.AspNetCore.Http;

namespace Shared.Helpers;

public static class EndpointResponse
{
    public static IResult Result<T>(Response<T> response)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.NotFound:
                return Microsoft.AspNetCore.Http.Results.NotFound(response);
            case HttpStatusCode.BadRequest:
                return Microsoft.AspNetCore.Http.Results.BadRequest(response);
            case HttpStatusCode.InternalServerError:
                return Microsoft.AspNetCore.Http.Results.InternalServerError(response);
            case HttpStatusCode.Unauthorized:
                return Microsoft.AspNetCore.Http.Results.Unauthorized();
            case HttpStatusCode.UnprocessableEntity:
                return Microsoft.AspNetCore.Http.Results.UnprocessableEntity(response);
            case HttpStatusCode.Created:
                return Microsoft.AspNetCore.Http.Results.Created(response.Meta.ToString(), response);
            default:
                return Microsoft.AspNetCore.Http.Results.Ok(response);
        }

    }

}
