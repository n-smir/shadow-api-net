﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShadowApiNet.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpPatchHandler : IHttpMethodHandler
    {
        internal override async Task<HttpContext> Handle(HttpContext httpContext, string rootUriPath, string[] pathNodes, DbContext dbContext, Dictionary<PropertyInfo, Type> dbSets, Dictionary<PropertyInfo, PropertyInfo[]> tablesFields)
        {
            if (pathNodes.Length == 2) { //entity name + id
                var pair = dbSets.Where(kvp => kvp.Key.Name.ToUpper() == pathNodes[0].ToUpper()).FirstOrDefault();
                object res = await dbContext.FindAsync(pair.Value, int.Parse(pathNodes[1])); // TODO: detect Id Type and convert to proper type
                JsonPatchDocument patchDoc = (JsonPatchDocument)JsonConvert.DeserializeObject(await new StreamReader(httpContext.Request.Body).ReadToEndAsync(), typeof(JsonPatchDocument));
                if (patchDoc == null) {
                    this.SetStatusCode(httpContext.Response, StatusCodes.Status400BadRequest);
                }
                if (res == null) {
                    this.SetStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                }

                patchDoc.ApplyTo(res);

                dbContext.Update(res);
                await dbContext.SaveChangesAsync();
                this.SetStatusCode(httpContext.Response, StatusCodes.Status204NoContent);
            }

            return httpContext;
        }
    }
}