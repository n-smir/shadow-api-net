using Microsoft.AspNetCore.Http;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Utils;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpHeadHandler(DbAccessor dbAccessor) : IHttpMethodHandler(dbAccessor)
    {
        internal override async Task<HttpContext> Handle(HttpContext httpContext, string rootUriPath, string[] pathNodes)
        {
            httpContext.Response.ContentType = "application/json";

            if (pathNodes.Length == 0 || string.IsNullOrEmpty(pathNodes[0]))
            {
                SetResponseStatusCode(httpContext.Response, StatusCodes.Status200OK);
            }
            else if (pathNodes.Length == 1)
            {
                var entityName = pathNodes[0];
                try
                {
                    var tableNames = _dbAccessor.GetTableNames();
                    if (tableNames.Contains(entityName.ToLower()))
                    {
                        SetResponseStatusCode(httpContext.Response, StatusCodes.Status200OK);
                    }
                    else
                    {
                        SetResponseStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                    }
                }
                catch
                {
                    SetResponseStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                }
            }
            else if (pathNodes.Length == 2)
            {
                var entityName = pathNodes[0];
                var entityId = pathNodes[1];
                try
                {
                    var result = await _dbAccessor.GetObjectByPrimaryKey(entityName, entityId);
                    SetResponseStatusCode(httpContext.Response,
                        result != null ? StatusCodes.Status200OK : StatusCodes.Status404NotFound);
                }
                catch
                {
                    SetResponseStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                }
            }
            else
            {
                SetResponseStatusCode(httpContext.Response, StatusCodes.Status400BadRequest);
            }

            return httpContext;
        }
    }
}
