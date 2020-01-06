using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ShadowApiNet.Dto;
using ShadowApiNet.Models;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShadowApiNet.Abstractions
{
    internal abstract class IHttpMethodHandler
    { 
        internal abstract Task<HttpContext> Handle(HttpContext httpContext, string rootUriPath, string[] pathNodes, DbContext dbContext, Dictionary<PropertyInfo, TableModel> tables);

        public HttpResponse SetStatusCode(HttpResponse response, int statusCode)
        {
            response.StatusCode = statusCode;
            return response;
        }

        public async Task<HttpResponse> SetJsonBody(HttpResponse response, ResponseWithLinksDto body)
        {
            string jsonBody = JsonConvert.SerializeObject(body, Formatting.Indented, new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            });
            await response.WriteAsync(jsonBody, Encoding.UTF8);
            return response;
        }
    }
}
