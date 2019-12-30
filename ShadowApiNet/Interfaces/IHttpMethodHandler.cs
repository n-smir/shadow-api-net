using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ShadowApiNet.Interfaces
{
    internal interface IHttpMethodHandler
    {
        Task<HttpContext> Handle(HttpContext httpContext);
    }
}
