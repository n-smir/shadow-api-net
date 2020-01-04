using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShadowApiNet.Dto;
using ShadowApiNet.Abstractions;
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
        public IHttpHandlerFactory HttpHandlerFactory { get; protected set; }

        private readonly Dictionary<PropertyInfo, Type> dbSets;
        private readonly Dictionary<PropertyInfo, PropertyInfo[]> tablesFields;

        public ApiResolver(DbContext context, IHttpHandlerFactory httpHandlerFactory, string rootUriPath)
        {
            this.RootUriPath = rootUriPath;
            this.Context = context;
            this.HttpHandlerFactory = httpHandlerFactory;

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

            IHttpMethodHandler methodHandler = this.HttpHandlerFactory.GetHttpMethodHandler(httpContext.Request.Method);
            httpContext = await methodHandler.Handle(httpContext, this.RootUriPath, pathNodes, this.Context, this.dbSets, this.tablesFields);

            return httpContext;
        }
    }
}