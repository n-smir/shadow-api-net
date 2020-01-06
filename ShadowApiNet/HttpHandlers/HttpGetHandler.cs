using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShadowApiNet.Dto;
using ShadowApiNet.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ShadowApiNet.Models;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpGetHandler : IHttpMethodHandler
    {

        internal override async Task<HttpContext> Handle(HttpContext httpContext, string rootUriPath, string[] pathNodes, DbContext dbContext, Dictionary<PropertyInfo, TableModel> tables)
        {
            string requestPath = httpContext.Request.Scheme + "://" + httpContext.Request.Host.Value + httpContext.Request.Path.Value + "/";

            ResponseWithLinksDto response = new ResponseWithLinksDto();

            if (pathNodes.Length == 0 || string.IsNullOrEmpty(pathNodes[0])) { //root path only
                foreach (var prop in tables.Keys) {
                    response.Links.Add(new LinkDto(requestPath + prop.Name.ToLower(), "self", "GET"));
                    response.Links.Add(new LinkDto(requestPath + prop.Name.ToLower(), "create", "POST"));
                }
                this.SetStatusCode(httpContext.Response, StatusCodes.Status200OK);
                await this.SetJsonBody(httpContext.Response, response);
            }
            else if (pathNodes.Length == 1) { //entity name only
                PropertyInfo tablePropInfo = tables.Keys.Where(k => k.Name.ToUpper() == pathNodes[0].ToUpper()).FirstOrDefault();
                IQueryable result = (IQueryable)tablePropInfo?.GetValue(dbContext);
                if (result != null) {
                    response.Value = new List<ResponseWithLinksDto>();
                    TableModel table = tables[tablePropInfo];
                    foreach(object obj in result) {
                        var id = table.PK?.GetValue(obj) ?? "{id}";
                        ((List<ResponseWithLinksDto>)response.Value).Add(new ResponseWithLinksDto {
                            Value = obj,
                            Links = new List<LinkDto> {
                                new LinkDto(requestPath + id, "self", "GET"),
                                new LinkDto(requestPath + id, "update", "PUT"),
                                new LinkDto(requestPath + id, "partial-update", "PATCH"),
                                new LinkDto(requestPath + id, "delete", "DELETE")
                            }
                        });
                    }
                    response.Links = new List<LinkDto>() {
                        new LinkDto(requestPath, "self", "GET"),
                        new LinkDto(requestPath, "create", "POST"),
                    };

                    this.SetStatusCode(httpContext.Response, StatusCodes.Status200OK);
                    await this.SetJsonBody(httpContext.Response, response);
                }
                else {
                    this.SetStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                }
            }
            else if (pathNodes.Length == 2) { //entity name + id
                var pair = tables.Where(kvp => kvp.Key.Name.ToUpper() == pathNodes[0].ToUpper()).FirstOrDefault();
                var res = await dbContext.FindAsync(pair.Value.Type, Convert.ChangeType(pathNodes[1], pair.Value.PK.PropertyType));
                if (res != null) {
                    response.Value = res;
                    response.Links = new List<LinkDto>() {
                            new LinkDto(requestPath, "self", "GET"),
                            new LinkDto(requestPath, "update", "PUT"),
                            new LinkDto(requestPath, "partial-update", "PATCH"),
                            new LinkDto(requestPath, "delete", "DELETE")
                    };
                    this.SetStatusCode(httpContext.Response, StatusCodes.Status200OK);
                    await this.SetJsonBody(httpContext.Response, response);
                }
                else {
                    this.SetStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                }
            }

            return httpContext;
        }
    }
}
