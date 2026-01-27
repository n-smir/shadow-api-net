using Microsoft.AspNetCore.Http;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowApiNet
{
    internal class ApiResolver(IHttpHandlerFactory httpHandlerFactory, string rootUriPath) : IApiResolver
    {
        public string RootUriPath { get; protected set; } = rootUriPath;
        public IHttpHandlerFactory HttpHandlerFactory { get; protected set; } = httpHandlerFactory;

        public async Task<HttpContext> ResolveRequest(HttpContext httpContext)
        {
            PathInfo pathInfo = new([.. httpContext.Request.Path.Value.Split('/').Where((node) => node != string.Empty && node != RootUriPath)]);

            IHttpMethodHandler methodHandler = HttpHandlerFactory.GetHttpMethodHandler(httpContext.Request.Method);
            httpContext = await methodHandler.Handle(httpContext, pathInfo);

            return httpContext;
        }
    }
}