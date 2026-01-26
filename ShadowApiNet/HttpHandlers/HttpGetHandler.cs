using Microsoft.AspNetCore.Http;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Dto;
using ShadowApiNet.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpGetHandler(DbAccessor dbAccessor) : IHttpMethodHandler(dbAccessor)
    {
        internal override async Task<HttpContext> Handle(HttpContext httpContext, string rootUriPath, string[] pathNodes)
        {
            string requestPath = httpContext.Request.Scheme + "://" + httpContext.Request.Host.Value + httpContext.Request.Path.Value + "/";

            ResponseWithLinksDto response = new ResponseWithLinksDto();

            if (pathNodes.Length == 0 || string.IsNullOrEmpty(pathNodes[0]))
            { //root path only
                foreach (var tableName in _dbAccessor.GetTableNames())
                {
                    response.Links.Add(new LinkDto(requestPath + tableName, "self", HttpMethods.Get));
                    response.Links.Add(new LinkDto(requestPath + tableName, "create", HttpMethods.Post));
                }
                SetResponseStatusCode(httpContext.Response, StatusCodes.Status200OK);
                await SetJsonBody(httpContext.Response, response);
            }
            else if (pathNodes.Length == 1)
            { //entity name only
                var entityName = pathNodes[0];
                var result = await _dbAccessor.GetAllObjectInRange(entityName, 0, 0);
                if (result != null)
                {
                    response.Value = new List<ResponseWithLinksDto>();
                    var tablePK = _dbAccessor.GetTablePrimaryKey(entityName);
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
                }
            }
            else if (pathNodes.Length == 2)
            { //entity name + id
                var entityName = pathNodes[0];
                var entityId = pathNodes[1];
                // var pair = tables.Where(kvp => kvp.Key.Name.Equals(entityName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                // var res = await dbContext.FindAsync(pair.Value.Type, Convert.ChangeType(entityId, pair.Value.PK.PropertyType));
                var result = await _dbAccessor.GetObjectByPrimaryKey(entityName, entityId);
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
                }
            }

            return httpContext;
        }
    }
}
