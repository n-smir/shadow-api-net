using Microsoft.AspNetCore.Http;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Utils;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpDeleteHandler(DbAccessor dbAccessor) : IHttpMethodHandler(dbAccessor)
    {
        internal override async Task<HttpContext> Handle(HttpContext httpContext, string rootUriPath, string[] pathNodes)
        {
            if (pathNodes.Length == 2)
            { //entity name + id
                var entityName = pathNodes[0];
                var entityId = pathNodes[1];
                var result = await _dbAccessor.GetObjectByPrimaryKey(entityName, entityId);
                if (result != null)
                {
                    await _dbAccessor.DeleteObject(result);
                    SetResponseStatusCode(httpContext.Response, StatusCodes.Status204NoContent);
                }
                else
                {
                    SetResponseStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                }
            }
            return httpContext;
        }
    }
}
