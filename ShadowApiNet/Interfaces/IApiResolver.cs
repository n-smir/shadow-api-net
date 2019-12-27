using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ShadowApiNet.Interfaces
{
    internal interface IApiResolver
    {
        string RootUriPath { get; }
        DbContext Context { get; }
        Task<HttpContext> ResolveRequest(HttpContext httpContext);
    }
}
