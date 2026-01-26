using Microsoft.AspNetCore.Http;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Utils;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpOptionsHandler(DbAccessor dbAccessor) : IHttpMethodHandler(dbAccessor)
    {
        internal override Task<HttpContext> Handle(HttpContext httpContext, string rootUriPath, string[] pathNodes)
        {
            httpContext.Response.ContentType = "application/json";

            string allowedMethods;
            if (pathNodes.Length == 0 || string.IsNullOrEmpty(pathNodes[0]))
            {
                allowedMethods = "GET, OPTIONS";
            }
            else if (pathNodes.Length == 1)
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
