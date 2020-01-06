using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpOptionsHandler : IHttpMethodHandler
    {
        internal override Task<HttpContext> Handle(HttpContext httpContext, string rootUriPath, string[] pathNodes, DbContext dbContext, Dictionary<PropertyInfo, TableModel> tables)
        {
            this.SetStatusCode(httpContext.Response, StatusCodes.Status501NotImplemented);
            return Task.Run(() => httpContext);
        }
    }
}
