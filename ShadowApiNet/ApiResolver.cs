using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ShadowApiNet
{
    internal class ApiResolver : IApiResolver
    {
        public string RootUriPath { get; protected set; }
        public DbContext Context { get; protected set; }
        public IHttpHandlerFactory HttpHandlerFactory { get; protected set; }

        private readonly Dictionary<PropertyInfo, TableModel> tables;

        public ApiResolver(DbContext context, IHttpHandlerFactory httpHandlerFactory, string rootUriPath)
        {
            this.RootUriPath = rootUriPath;
            this.Context = context;
            this.HttpHandlerFactory = httpHandlerFactory;

            this.tables = new Dictionary<PropertyInfo, TableModel>();
            foreach (PropertyInfo prop in this.Context.GetType().GetProperties())
            {
                if (prop.PropertyType.Name.Contains("DbSet"))
                {
                    var type = prop.PropertyType.GetGenericArguments().FirstOrDefault();

                    this.tables.Add(prop, new TableModel
                    {
                        Type = type,
                        Fields = type.GetProperties(),
                        PK = type.GetProperties().Where(p => p.CustomAttributes.Where(ca => ca.AttributeType.Name == "KeyAttribute").Any()).FirstOrDefault()
                    });
                }
            }
        }

        public async Task<HttpContext> ResolveRequest(HttpContext httpContext)
        {
            string[] pathNodes = httpContext.Request.Path.Value.Trim(this.RootUriPath.ToCharArray()).Split('/');

            IHttpMethodHandler methodHandler = this.HttpHandlerFactory.GetHttpMethodHandler(httpContext.Request.Method);
            httpContext = await methodHandler.Handle(httpContext, this.RootUriPath, pathNodes, this.Context, this.tables);

            return httpContext;
        }
    }
}