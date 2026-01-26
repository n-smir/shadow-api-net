using Microsoft.AspNetCore.Http;
using ShadowApiNet.Abstractions;
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
            string[] pathNodes = [.. httpContext.Request.Path.Value.Split('/').Where((node) => node != string.Empty && node != this.RootUriPath)];

            IHttpMethodHandler methodHandler = this.HttpHandlerFactory.GetHttpMethodHandler(httpContext.Request.Method);
            httpContext = await methodHandler.Handle(httpContext, this.RootUriPath, pathNodes);

            return httpContext;
        }
    }
}