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
    internal class HttpPutHandler : IHttpMethodHandler
    {
        internal override async Task<HttpContext> Handle(HttpContext httpContext, string rootUriPath, string[] pathNodes, DbContext dbContext, Dictionary<PropertyInfo, TableModel> tables)
        {
            if (pathNodes.Length == 2)
            { //entity name + id
                var pair = tables.Where(kvp => kvp.Key.Name.ToUpper() == pathNodes[0].ToUpper()).FirstOrDefault();
                object res = await dbContext.FindAsync(pair.Value.Type, Convert.ChangeType(pathNodes[1], pair.Value.PK.PropertyType)); // TODO: detect Id Type and convert to proper type
                object body = JsonConvert.DeserializeObject(await new StreamReader(httpContext.Request.Body).ReadToEndAsync(), pair.Value.Type);
                //props comparison
                foreach (var propInfo in tables[pair.Key].Fields)
                {
                    if (propInfo.GetValue(res) != propInfo.GetValue(body))
                    {
                        propInfo.SetValue(res, propInfo.GetValue(body));
                    }
                }
                dbContext.Update(res);
                await dbContext.SaveChangesAsync();
                this.SetStatusCode(httpContext.Response, StatusCodes.Status204NoContent);
            }

            return httpContext;
        }
    }
}
