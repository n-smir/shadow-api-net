using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpPostHandler : IHttpMethodHandler
    {
        internal override async Task<HttpContext> Handle(HttpContext httpContext, string rootUriPath, string[] pathNodes, DbContext dbContext, Dictionary<PropertyInfo, TableModel> tables)
        {
            if (pathNodes.Length == 1) { //entity name only
                    var propType = tables.Where(pair => pair.Key.Name.ToUpper() == pathNodes[0].ToUpper()).First();
                    object body = JsonConvert.DeserializeObject(await new StreamReader(httpContext.Request.Body).ReadToEndAsync(), propType.Value.Type);
                    await dbContext.AddAsync(body);
                    await dbContext.SaveChangesAsync();
                    this.SetStatusCode(httpContext.Response, StatusCodes.Status201Created);
            }

            return httpContext;
        }
    }
}
