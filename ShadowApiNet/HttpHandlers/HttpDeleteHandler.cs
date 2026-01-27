using Microsoft.AspNetCore.Http;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Models;
using ShadowApiNet.Utils;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpDeleteHandler(DbAccessor dbAccessor) : IHttpMethodHandler(dbAccessor)
    {
        internal override async Task<HttpContext> Handle(HttpContext httpContext, PathInfo pathInfo)
        {
            if (pathInfo.IsResource)
            {
                var result = await _dbAccessor.GetObjectByPrimaryKey(pathInfo.EntityName, pathInfo.EntityId);
                if (result != null)
                {
                    await _dbAccessor.DeleteObject(result);
                    SetResponseStatusCode(httpContext.Response, StatusCodes.Status204NoContent);
                }
                else
                {
                    SetResponseStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                    await SetErrorBody(httpContext.Response, "Not Found", "The requested resource was not found");
                }
            }
            return httpContext;
        }
    }
}
