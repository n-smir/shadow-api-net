using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Utils;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpPostHandler(DbAccessor dbAccessor) : IHttpMethodHandler(dbAccessor)
    {
        internal override async Task<HttpContext> Handle(HttpContext httpContext, string rootUriPath, string[] pathNodes)
        {
            if (pathNodes.Length == 1)
            { //entity name only
                string objectName = pathNodes[0];
                Type objectType = _dbAccessor.ResolveObjectType(objectName);
                object body = JsonConvert.DeserializeObject(await new StreamReader(httpContext.Request.Body).ReadToEndAsync(), objectType);
                await _dbAccessor.CreateObject(body);
                SetResponseStatusCode(httpContext.Response, StatusCodes.Status201Created);
            }
            else
            {
                throw new NotImplementedException();
            }

            return httpContext;
        }
    }
}
