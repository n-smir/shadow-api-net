using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Models;
using ShadowApiNet.Utils;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpPostHandler(DbAccessor dbAccessor) : IHttpMethodHandler(dbAccessor)
    {
        internal override async Task<HttpContext> Handle(HttpContext httpContext, PathInfo pathInfo)
        {
            if (pathInfo.IsRoot)
            {
                SetResponseStatusCode(httpContext.Response, StatusCodes.Status400BadRequest);
                await SetErrorBody(httpContext.Response, "Bad Request", "Entity name is required");
            }
            else if (pathInfo.IsCollection)
            {
                Type objectType = _dbAccessor.ResolveObjectType(pathInfo.EntityName);
                object body = JsonConvert.DeserializeObject(await new StreamReader(httpContext.Request.Body).ReadToEndAsync(), objectType);

                if (body == null)
                {
                    SetResponseStatusCode(httpContext.Response, StatusCodes.Status400BadRequest);
                    await SetErrorBody(httpContext.Response, "Bad Request", "Request body is required and must be valid JSON");
                    return httpContext;
                }

                await _dbAccessor.CreateObject(body);
                SetResponseStatusCode(httpContext.Response, StatusCodes.Status201Created);
            }
            else
            {
                httpContext.Response.Headers.Add("Allow", "GET, PUT, PATCH, DELETE, OPTIONS, HEAD");
                SetResponseStatusCode(httpContext.Response, StatusCodes.Status405MethodNotAllowed);
                await SetErrorBody(httpContext.Response, "Method Not Allowed", "POST is not allowed on specific resources");
            }

            return httpContext;
        }
    }
}
