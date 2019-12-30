using Microsoft.AspNetCore.Http;
using ShadowApiNet.Interfaces;
using System;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpDeleteHandler : IHttpMethodHandler
    {
        public Task<HttpContext> Handle(HttpContext httpContext)
        {
            throw new NotImplementedException();
        }
    }
}
