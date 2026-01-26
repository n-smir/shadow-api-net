using Microsoft.AspNetCore.Http;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Models;
using ShadowApiNet.Utils;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpOptionsHandler(DbAccessor dbAccessor) : IHttpMethodHandler(dbAccessor)
    {
        internal override Task<HttpContext> Handle(HttpContext httpContext, PathInfo pathInfo)
        {
            httpContext.Response.ContentType = "application/json";

            string allowedMethods;
            if (pathInfo.IsRoot)
            {
                allowedMethods = "GET, OPTIONS";
            }
            else if (pathInfo.IsCollection)
            {
                allowedMethods = "GET, POST, OPTIONS, HEAD";
            }
            else
            {
                allowedMethods = "GET, PUT, PATCH, DELETE, OPTIONS, HEAD";
            }

            httpContext.Response.Headers.Add("Allow", allowedMethods);
            SetResponseStatusCode(httpContext.Response, StatusCodes.Status200OK);

            return Task.FromResult(httpContext);
        }
    }
}
