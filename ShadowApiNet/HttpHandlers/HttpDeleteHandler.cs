using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpDeleteHandler : IHttpMethodHandler
    {
        internal override async Task<HttpContext> Handle(HttpContext httpContext, string rootUriPath, string[] pathNodes, DbContext dbContext, Dictionary<PropertyInfo, TableModel> tables)
        {
            if (pathNodes.Length == 2) { //entity name + id
                    var pair = tables.Where(kvp => kvp.Key.Name.ToUpper() == pathNodes[0].ToUpper()).FirstOrDefault();
                    var res = await dbContext.FindAsync(pair.Value.Type, Convert.ChangeType(pathNodes[1], pair.Value.PK.PropertyType));
                    dbContext.Remove(res);
                    await dbContext.SaveChangesAsync();
                    this.SetStatusCode(httpContext.Response, StatusCodes.Status204NoContent);
            }
            return httpContext;
        }
    }
}
