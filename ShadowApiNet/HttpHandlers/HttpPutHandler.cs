using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Utils;
using System.IO;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpPutHandler(DbAccessor dbAccessor) : IHttpMethodHandler(dbAccessor)
    {
        internal override async Task<HttpContext> Handle(HttpContext httpContext, string rootUriPath, string[] pathNodes)
        {
            if (pathNodes.Length == 2)
            { //entity name + id
                var entityName = pathNodes[0];
                var entityId = pathNodes[1];
                var existingObject = await _dbAccessor.GetObjectByPrimaryKey(entityName, entityId);
                if (existingObject == null)
                {
                    SetResponseStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                    return httpContext;
                }

                var objectType = _dbAccessor.ResolveObjectType(entityName);
                var bodyObject = JsonConvert.DeserializeObject(await new StreamReader(httpContext.Request.Body).ReadToEndAsync(), objectType);
                var metadata = _dbAccessor.ResolveObjectMetadata(entityName);

                //props comparison
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
