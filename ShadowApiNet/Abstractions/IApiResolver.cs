using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ShadowApiNet.Abstractions
{
    internal interface IApiResolver
    {
        string RootUriPath { get; }
        Task<HttpContext> ResolveRequest(HttpContext httpContext);
    }
}
