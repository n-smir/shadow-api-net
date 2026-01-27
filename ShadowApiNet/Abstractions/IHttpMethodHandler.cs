using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ShadowApiNet.Dto;
using ShadowApiNet.Models;
using ShadowApiNet.Utils;
using System.Text;
using System.Threading.Tasks;

namespace ShadowApiNet.Abstractions
{
    internal abstract class IHttpMethodHandler(DbAccessor dbAccessor)
    {
        protected readonly DbAccessor _dbAccessor = dbAccessor;

        internal abstract Task<HttpContext> Handle(HttpContext httpContext, PathInfo pathInfo);

        protected static void SetResponseStatusCode(HttpResponse response, int statusCode)
        {
            response.StatusCode = statusCode;
        }

        protected static async Task SetJsonBody(HttpResponse response, ResponseWithLinksDto body)
        {
            string jsonBody = JsonConvert.SerializeObject(body, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            });
            await response.WriteAsync(jsonBody, Encoding.UTF8);
        }

        protected static async Task SetErrorBody(HttpResponse response, string error, string message)
        {
            response.ContentType = "application/json";
            var errorObject = new
            {
                error,
                message,
                statusCode = response.StatusCode
            };
            string jsonBody = JsonConvert.SerializeObject(errorObject, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            await response.WriteAsync(jsonBody, Encoding.UTF8);
        }
    }
}
