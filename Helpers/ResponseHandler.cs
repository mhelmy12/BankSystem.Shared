using System;
using System.Net;

namespace Shared.Helpers;

public class ResponseHandler
{
     public ResponseHandler()
    {

    }
    public Response<T> Deleted<T>(string msg = "Deleted Successfully")
    {
        return new Response<T>()
        {
            StatusCode = HttpStatusCode.OK,
            Succeeded = true,
            Message = msg
        };
    }

    public Response<T> Created<T>(T entity, string msg = "Created Successfully", object Meta = null)
    {
        return new Response<T>()
        {
            Data = entity,
            StatusCode = HttpStatusCode.Created,
            Succeeded = true,
            Message = msg,
            Meta = Meta
        };
    }
    public Response<T> Success<T>(T entity, string msg = "Retrieved Successfully", object Meta = null)
    {
        return new Response<T>()
        {
            Data = entity,
            StatusCode = HttpStatusCode.OK,
            Succeeded = true,
            Message = msg,
            Meta = Meta
        };
    }
    public Response<T> Unauthorized<T>(string msg = "UnAuthorized")
    {
        return new Response<T>()
        {
            StatusCode = HttpStatusCode.Unauthorized,
            Succeeded = false,
            Message = msg
        };
    }
    public Response<T> BadRequest<T>(string Message = null)
    {
        return new Response<T>()
        {
            StatusCode = HttpStatusCode.BadRequest,
            Succeeded = false,
            Message = Message == null ? "Bad Request" : Message
        };
    }


    public Response<T> Unproccess<T>(string Message = null)
    {
        return new Response<T>()
        {
            StatusCode = HttpStatusCode.UnprocessableEntity,
            Succeeded = false,
            Message = Message == null ? "Unprocessable Entity" : Message
        };
    }

    public Response<T> NotFound<T>(string message = null)
    {
        return new Response<T>()
        {
            StatusCode = HttpStatusCode.NotFound,
            Succeeded = false,
            Message = message == null ? "Not Found" : message
        };
    }

    public Response<T> Created<T>(T entity, object Meta = null)
    {
        return new Response<T>()
        {
            Data = entity,
            StatusCode = HttpStatusCode.Created,
            Succeeded = true,
            Message = "Created",
            Meta = Meta
        };
    }

}
