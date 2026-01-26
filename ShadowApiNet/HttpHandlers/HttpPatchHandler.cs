using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Utils;
using System.IO;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpPatchHandler(DbAccessor dbAccessor) : IHttpMethodHandler(dbAccessor)
    {
        internal override async Task<HttpContext> Handle(HttpContext httpContext, string rootUriPath, string[] pathNodes)
        {
            if (pathNodes.Length == 2)
            { //entity name + id
                var entityName = pathNodes[0];
                var entityId = pathNodes[1];
                var existingObject = await _dbAccessor.GetObjectByPrimaryKey(entityName, entityId);

                JsonPatchDocument patchDoc = (JsonPatchDocument)JsonConvert.DeserializeObject(await new StreamReader(httpContext.Request.Body).ReadToEndAsync(), typeof(JsonPatchDocument));
                if (patchDoc == null)
                {
                    SetResponseStatusCode(httpContext.Response, StatusCodes.Status400BadRequest);
                    await SetErrorBody(httpContext.Response, "Bad Request", "Request body is required and must be valid JSON Patch format");
                    return httpContext;
                }
                if (existingObject == null)
                {
                    SetResponseStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                    await SetErrorBody(httpContext.Response, "Not Found", "The requested resource was not found");
                    return httpContext;
                }

                patchDoc.ApplyTo(existingObject);

                await _dbAccessor.UpdateObject(existingObject);
                SetResponseStatusCode(httpContext.Response, StatusCodes.Status204NoContent);
            }

            return httpContext;
        }
    }
}
