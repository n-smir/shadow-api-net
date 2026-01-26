using Microsoft.AspNetCore.Http;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Models;
using ShadowApiNet.Utils;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpHeadHandler(DbAccessor dbAccessor) : IHttpMethodHandler(dbAccessor)
    {
        internal override async Task<HttpContext> Handle(HttpContext httpContext, PathInfo pathInfo)
        {
            httpContext.Response.ContentType = "application/json";

            if (pathInfo.IsRoot)
            {
                SetResponseStatusCode(httpContext.Response, StatusCodes.Status200OK);
            }
            else if (pathInfo.IsCollection)
            {
                try
                {
                    var tableNames = _dbAccessor.GetTableNames();
                    if (tableNames.Contains(pathInfo.EntityName.ToLower()))
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
            else if (pathInfo.IsResource)
            {
                try
                {
                    var result = await _dbAccessor.GetObjectByPrimaryKey(pathInfo.EntityName, pathInfo.EntityId);
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
