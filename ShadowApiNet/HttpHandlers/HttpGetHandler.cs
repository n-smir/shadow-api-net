using Microsoft.AspNetCore.Http;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Dto;
using ShadowApiNet.Models;
using ShadowApiNet.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpGetHandler(DbAccessor dbAccessor) : IHttpMethodHandler(dbAccessor)
    {
        internal override async Task<HttpContext> Handle(HttpContext httpContext, PathInfo pathInfo)
        {
            string requestPath = httpContext.Request.Scheme + "://" + httpContext.Request.Host.Value + httpContext.Request.Path.Value + "/";

            ResponseWithLinksDto response = new ResponseWithLinksDto();

            if (pathInfo.IsRoot)
            {
                foreach (var tableName in _dbAccessor.GetTableNames())
                {
                    response.Links.Add(new LinkDto(requestPath + tableName, "self", HttpMethods.Get));
                    response.Links.Add(new LinkDto(requestPath + tableName, "create", HttpMethods.Post));
                }
                SetResponseStatusCode(httpContext.Response, StatusCodes.Status200OK);
                await SetJsonBody(httpContext.Response, response);
            }
            else if (pathInfo.IsCollection)
            {
                var result = await _dbAccessor.GetAllObjectInRange(pathInfo.EntityName, 0, 0);
                if (result != null)
                {
                    response.Value = new List<ResponseWithLinksDto>();
                    var tablePK = _dbAccessor.GetTablePrimaryKey(pathInfo.EntityName);
                    foreach (object obj in result)
                    {
                        var id = tablePK?.GetValue(obj) ?? "{id}";
                        ((List<ResponseWithLinksDto>)response.Value).Add(new ResponseWithLinksDto
                        {
                            Value = obj,
                            Links = LinkBuilder.CreateLinksForObject(requestPath, id.ToString())
                        });
                    }
                    response.Links = LinkBuilder.CreateLinksForList(requestPath);

                    SetResponseStatusCode(httpContext.Response, StatusCodes.Status200OK);
                    await SetJsonBody(httpContext.Response, response);
                }
                else
                {
                    SetResponseStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                    await SetErrorBody(httpContext.Response, "Not Found", "The requested resource was not found");
                }
            }
            else if (pathInfo.IsResource)
            {
                var result = await _dbAccessor.GetObjectByPrimaryKey(pathInfo.EntityName, pathInfo.EntityId);
                if (result != null)
                {
                    response.Value = result;
                    response.Links = LinkBuilder.CreateLinksForObject(requestPath);
                    SetResponseStatusCode(httpContext.Response, StatusCodes.Status200OK);
                    await SetJsonBody(httpContext.Response, response);
                }
                else
                {
                    SetResponseStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                    await SetErrorBody(httpContext.Response, "Not Found", "The requested resource was not found");
                }
            }

            return httpContext;
        }
    }
}
