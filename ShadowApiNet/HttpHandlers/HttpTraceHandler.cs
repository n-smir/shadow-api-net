using Microsoft.AspNetCore.Http;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Models;
using ShadowApiNet.Utils;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpTraceHandler(DbAccessor dbAccessor) : IHttpMethodHandler(dbAccessor)
    {
        internal override Task<HttpContext> Handle(HttpContext httpContext, PathInfo pathInfo)
        {
            SetResponseStatusCode(httpContext.Response, StatusCodes.Status501NotImplemented);
            return Task.Run(() => httpContext);
        }
    }
}
