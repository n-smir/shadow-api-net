using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShadowApiNet.Dto;
using ShadowApiNet.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShadowApiNet
{
    internal class ApiResolver : IApiResolver
    {
        public string RootUriPath { get; protected set; }
        public DbContext Context { get; protected set; }

        private readonly Dictionary<PropertyInfo, Type> dbSets;
        private readonly Dictionary<PropertyInfo, PropertyInfo[]> tablesFields;

        public ApiResolver(DbContext context, string rootUriPath)
        {
            this.RootUriPath = rootUriPath;
            this.Context = context;

            this.dbSets = new Dictionary<PropertyInfo, Type>();
            this.tablesFields = new Dictionary<PropertyInfo, PropertyInfo[]>();
            foreach (PropertyInfo prop in this.Context.GetType().GetProperties()) {
                if (prop.PropertyType.Name.Contains("DbSet")) {
                    var type = prop.PropertyType.GetGenericArguments().FirstOrDefault();
                    this.dbSets.Add(prop, type);
                    this.tablesFields.Add(prop, type.GetProperties());
                }
            }
        }

        public async Task<HttpContext> ResolveRequest(HttpContext httpContext)
        {
            string[] pathNodes = httpContext.Request.Path.Value.Trim(this.RootUriPath.ToCharArray()).Split('/');

            if (pathNodes.Length == 0 || string.IsNullOrEmpty(pathNodes[0])) { //root path only

                if (httpContext.Request.Method == HttpMethods.Get) {
                    List<Link> links = new List<Link>();
                    foreach (var prop in this.dbSets.Keys) {
                        Link link = new Link { href = httpContext.Request.PathBase.Value.TrimEnd('/') + "/" + this.RootUriPath.Trim('/') + "/" + prop.Name.ToLower() };
                        links.Add(link);
                    }
                    this.SetStatusCode(httpContext.Response, StatusCodes.Status200OK);
                    await this.SetJsonBody(httpContext.Response, links);
                }
                else {
                    this.SetStatusCode(httpContext.Response, StatusCodes.Status405MethodNotAllowed);
                }
            }
            else if (pathNodes.Length == 1) { //entity name only
                if (httpContext.Request.Method == HttpMethods.Get) {
                    var result = this.dbSets.Keys.Where(k => k.Name.ToUpper() == pathNodes[0].ToUpper()).FirstOrDefault()?.GetValue(this.Context);
                    if (result != null) {
                        this.SetStatusCode(httpContext.Response, StatusCodes.Status200OK);
                        await this.SetJsonBody(httpContext.Response, result);
                    }
                    else {
                        this.SetStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                    }
                }
                else if (httpContext.Request.Method == HttpMethods.Post) {
                    var propType = this.dbSets.Where(pair => pair.Key.Name.ToUpper() == pathNodes[0].ToUpper()).First();
                    object body = JsonConvert.DeserializeObject(await new StreamReader(httpContext.Request.Body).ReadToEndAsync(), propType.Value);
                    await this.Context.AddAsync(body);
                    await this.Context.SaveChangesAsync();
                    this.SetStatusCode(httpContext.Response, StatusCodes.Status201Created);
                }
                else {
                    this.SetStatusCode(httpContext.Response, StatusCodes.Status405MethodNotAllowed);
                }
            }
            else if (pathNodes.Length == 2) { //entity name + id
                if (httpContext.Request.Method == HttpMethods.Get) {
                    var pair = this.dbSets.Where(kvp => kvp.Key.Name.ToUpper() == pathNodes[0].ToUpper()).FirstOrDefault();
                    var res = await this.Context.FindAsync(pair.Value, int.Parse(pathNodes[1])); // TODO: detect Id Type and convert to proper type
                    if (res != null) {
                        this.SetStatusCode(httpContext.Response, StatusCodes.Status200OK);
                        await this.SetJsonBody(httpContext.Response, res);
                    }
                    else {
                        this.SetStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                    }
                }
                else if (httpContext.Request.Method == HttpMethods.Delete) {
                    var pair = this.dbSets.Where(kvp => kvp.Key.Name.ToUpper() == pathNodes[0].ToUpper()).FirstOrDefault();
                    var res = await this.Context.FindAsync(pair.Value, int.Parse(pathNodes[1])); // TODO: detect Id Type and convert to proper type
                    this.Context.Remove(res);
                    await this.Context.SaveChangesAsync();
                    this.SetStatusCode(httpContext.Response, StatusCodes.Status204NoContent);
                }
                else if (httpContext.Request.Method == HttpMethods.Put) {
                    var pair = this.dbSets.Where(kvp => kvp.Key.Name.ToUpper() == pathNodes[0].ToUpper()).FirstOrDefault();
                    object res = await this.Context.FindAsync(pair.Value, int.Parse(pathNodes[1])); // TODO: detect Id Type and convert to proper type
                    object body = JsonConvert.DeserializeObject(await new StreamReader(httpContext.Request.Body).ReadToEndAsync(), pair.Value);
                    //props comparison
                    foreach (var propInfo in tablesFields[pair.Key]) {
                        if (propInfo.GetValue(res) != propInfo.GetValue(body)) {
                            propInfo.SetValue(res, propInfo.GetValue(body));
                        }
                    }
                    this.Context.Update(res);
                    await this.Context.SaveChangesAsync();
                    this.SetStatusCode(httpContext.Response, StatusCodes.Status204NoContent);
                }
                else if (httpContext.Request.Method == HttpMethods.Patch) {
                    var pair = this.dbSets.Where(kvp => kvp.Key.Name.ToUpper() == pathNodes[0].ToUpper()).FirstOrDefault();
                    object res = await this.Context.FindAsync(pair.Value, int.Parse(pathNodes[1])); // TODO: detect Id Type and convert to proper type
                    JsonPatchDocument patchDoc = (JsonPatchDocument)JsonConvert.DeserializeObject(await new StreamReader(httpContext.Request.Body).ReadToEndAsync(), typeof(JsonPatchDocument));
                    if (patchDoc == null) {
                        this.SetStatusCode(httpContext.Response, StatusCodes.Status400BadRequest);
                    }
                    if(res == null) {
                        this.SetStatusCode(httpContext.Response, StatusCodes.Status404NotFound);
                    }

                    patchDoc.ApplyTo(res);

                    this.Context.Update(res);
                    await this.Context.SaveChangesAsync();
                    this.SetStatusCode(httpContext.Response, StatusCodes.Status204NoContent);
                }
                else {
                    this.SetStatusCode(httpContext.Response, StatusCodes.Status405MethodNotAllowed);
                }
            }
            else {
                this.SetStatusCode(httpContext.Response, StatusCodes.Status501NotImplemented);
            }

            return httpContext;
        }


        public HttpResponse SetStatusCode(HttpResponse response, int statusCode)
        {
            response.StatusCode = statusCode;
            return response;
        }

        public async Task<HttpResponse> SetJsonBody(HttpResponse response, object body)
        {
            string jsonBody = JsonConvert.SerializeObject(body, Formatting.Indented, new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            await response.WriteAsync(jsonBody, Encoding.UTF8);
            return response;
        }
    }
}