using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShadowApiNet.Dto;
using ShadowApiNet.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpGetHandler : IHttpMethodHandler
    {

        internal override async Task<HttpContext> Handle(HttpContext httpContext, string rootUriPath, string[] pathNodes, DbContext dbContext, Dictionary<PropertyInfo, Type> dbSets, Dictionary<PropertyInfo, PropertyInfo[]> tablesFields)
        {
            if (pathNodes.Length == 0 || string.IsNullOrEmpty(pathNodes[0])) { //root path only
                    List<Link> links = new List<Link>();
                    foreach (var prop in dbSets.Keys) {
                        Link link = new Link { href = httpContext.Request.PathBase.Value.TrimEnd('/') + "/" + rootUriPath.Trim('/') + "/" + prop.Name.ToLower() };
                        links.Add(link);
                    }
                    this.SetStatusCode(httpContext.Response, StatusCodes.Status200OK);
                    await this.SetJsonBody(httpContext.Response, links);
            }
            else if (pathNodes.Length == 1) { //entity name only
                    var result = dbSets.Keys.Where(k => k.Name.ToUpper() == pathNodes[0].ToUpper()).FirstOrDefault()?.GetValue(dbContext);
                    if (result != null) {
                        this.SetStatusCode(httpContext.Response, StatusCodes.Status200OK);
                        await this.SetJsonBody(httpContext.Response, result);
                    }
                    else {
                        this.SetStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                    }
            }
            else if (pathNodes.Length == 2) { //entity name + id
                    var pair = dbSets.Where(kvp => kvp.Key.Name.ToUpper() == pathNodes[0].ToUpper()).FirstOrDefault();
                    var res = await dbContext.FindAsync(pair.Value, int.Parse(pathNodes[1])); // TODO: detect Id Type and convert to proper type
                    if (res != null) {
                        this.SetStatusCode(httpContext.Response, StatusCodes.Status200OK);
                        await this.SetJsonBody(httpContext.Response, res);
                    }
                    else {
                        this.SetStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                    }
                }

                return httpContext;
        }
    }
}
