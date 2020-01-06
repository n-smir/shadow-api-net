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
            string requestPath = httpContext.Request.Scheme + "://" + httpContext.Request.Host.Value + httpContext.Request.Path.Value;

            ResponseWithLinksDto response = new ResponseWithLinksDto();

            if (pathNodes.Length == 0 || string.IsNullOrEmpty(pathNodes[0])) { //root path only
                foreach (var prop in dbSets.Keys) {
                    response.Links.Add(new LinkDto(requestPath + prop.Name.ToLower(), "self", "GET"));
                    response.Links.Add(new LinkDto(requestPath + prop.Name.ToLower(), "create-" + prop.Name.ToLower(), "POST"));
                }
                this.SetStatusCode(httpContext.Response, StatusCodes.Status200OK);
                await this.SetJsonBody(httpContext.Response, response);
            }
            else if (pathNodes.Length == 1) { //entity name only
                IQueryable result = (IQueryable)dbSets.Keys.Where(k => k.Name.ToUpper() == pathNodes[0].ToUpper()).FirstOrDefault()?.GetValue(dbContext);
                if (result != null) {
                    response.Value = new List<ResponseWithLinksDto>();
                    foreach(object obj in result) {
                        ((List<ResponseWithLinksDto>)response.Value).Add(new ResponseWithLinksDto {
                            Value = obj,
                            Links = new List<LinkDto> {
                                new LinkDto(requestPath + "/{id}", "self", "GET"),
                                new LinkDto(requestPath + "/{id}", "update", "PUT"),
                                new LinkDto(requestPath + "/{id}", "partial-update", "PATCH"),
                                new LinkDto(requestPath + "/{id}", "delete", "DELETE")
                            }
                        });
                    }
                    response.Links = new List<LinkDto>() {
                        new LinkDto(requestPath + pathNodes[0].ToLower(), "self", "GET"),
                        new LinkDto(requestPath + pathNodes[0].ToLower(), "create", "POST"),
                    };

                    this.SetStatusCode(httpContext.Response, StatusCodes.Status200OK);
                    await this.SetJsonBody(httpContext.Response, response);
                }
                else {
                    this.SetStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                }
            }
            else if (pathNodes.Length == 2) { //entity name + id
                var pair = dbSets.Where(kvp => kvp.Key.Name.ToUpper() == pathNodes[0].ToUpper()).FirstOrDefault();
                var res = await dbContext.FindAsync(pair.Value, int.Parse(pathNodes[1])); // TODO: detect Id Type and convert to proper type
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
