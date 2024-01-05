using Microsoft.AspNetCore.Http;
using ShadowApiNet.Abstractions;
using System.Threading.Tasks;

namespace ShadowApiNet
{
    internal class ApiResolverMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiResolverMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IApiResolver apiResolver)
        {
            if (!httpContext.Request.Path.Value.Contains(apiResolver.RootUriPath))
            {
                await _next.Invoke(httpContext);
                return;
            }

            httpContext = await apiResolver.ResolveRequest(httpContext);
        }
    }
}