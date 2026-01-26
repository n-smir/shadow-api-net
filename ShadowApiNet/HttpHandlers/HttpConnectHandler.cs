using Microsoft.AspNetCore.Http;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Utils;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpConnectHandler(DbAccessor dbAccessor) : IHttpMethodHandler(dbAccessor)
    {
        internal override Task<HttpContext> Handle(HttpContext httpContext, string rootUriPath, string[] pathNodes)
        {
            SetResponseStatusCode(httpContext.Response, StatusCodes.Status501NotImplemented);
            return Task.Run(() => httpContext);
        }
    }
}
