using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShadowApiNet.Abstractions
{
    internal abstract class IHttpMethodHandler
    { 
        internal abstract Task<HttpContext> Handle(HttpContext httpContext, string rootUriPath, string[] pathNodes, DbContext dbContext, Dictionary<PropertyInfo, Type> dbSets, Dictionary<PropertyInfo, PropertyInfo[]> tablesFields);

        public HttpResponse SetStatusCode(HttpResponse response, int statusCode)
        {
            response.StatusCode = statusCode;
            return response;
        }

        public async Task<HttpResponse> SetJsonBody(HttpResponse response, object body)
        {
            string jsonBody = JsonConvert.SerializeObject(body, Formatting.Indented, new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            await response.WriteAsync(jsonBody, Encoding.UTF8);
            return response;
        }
    }
}
