using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Models;
using ShadowApiNet.Utils;
using System.IO;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpPutHandler(DbAccessor dbAccessor) : IHttpMethodHandler(dbAccessor)
    {
        internal override async Task<HttpContext> Handle(HttpContext httpContext, PathInfo pathInfo)
        {
            if (pathInfo.IsResource)
            {
                var existingObject = await _dbAccessor.GetObjectByPrimaryKey(pathInfo.EntityName, pathInfo.EntityId);
                if (existingObject == null)
                {
                    SetResponseStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                    await SetErrorBody(httpContext.Response, "Not Found", "The requested resource was not found");
                    return httpContext;
                }

                var objectType = _dbAccessor.ResolveObjectType(pathInfo.EntityName);
                var bodyObject = JsonConvert.DeserializeObject(await new StreamReader(httpContext.Request.Body).ReadToEndAsync(), objectType);
                var metadata = _dbAccessor.ResolveObjectMetadata(pathInfo.EntityName);

                foreach (var propInfo in metadata.Value.Fields)
                {
                    if (propInfo.GetValue(existingObject) != propInfo.GetValue(bodyObject))
                    {
                        propInfo.SetValue(existingObject, propInfo.GetValue(bodyObject));
                    }
                }
                await _dbAccessor.UpdateObject(existingObject);
                SetResponseStatusCode(httpContext.Response, StatusCodes.Status204NoContent);
            }

            return httpContext;
        }
    }
}
