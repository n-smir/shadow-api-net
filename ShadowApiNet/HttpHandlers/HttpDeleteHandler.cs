using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShadowApiNet.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpDeleteHandler : IHttpMethodHandler
    {
        internal override async Task<HttpContext> Handle(HttpContext httpContext, string rootUriPath, string[] pathNodes, DbContext dbContext, Dictionary<PropertyInfo, Type> dbSets, Dictionary<PropertyInfo, PropertyInfo[]> tablesFields)
        {
            if (pathNodes.Length == 2) { //entity name + id
                    var pair = dbSets.Where(kvp => kvp.Key.Name.ToUpper() == pathNodes[0].ToUpper()).FirstOrDefault();
                    var res = await dbContext.FindAsync(pair.Value, int.Parse(pathNodes[1])); // TODO: detect Id Type and convert to proper type
                    dbContext.Remove(res);
                    await dbContext.SaveChangesAsync();
                    this.SetStatusCode(httpContext.Response, StatusCodes.Status204NoContent);
            }
            return httpContext;
        }
    }
}
