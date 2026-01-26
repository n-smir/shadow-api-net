using Microsoft.AspNetCore.Http;
using ShadowApiNet.Abstractions;
using ShadowApiNet.HttpHandlers;
using ShadowApiNet.Utils;
using System;
using System.Collections.Generic;

namespace ShadowApiNet
{
    internal class HttpMethodHandlerFactory(DbAccessor dbAccessor) : IHttpHandlerFactory
    {
        private readonly Dictionary<string, IHttpMethodHandler> _handlers = new Dictionary<string, IHttpMethodHandler>();
        private readonly DbAccessor _dbAccessor = dbAccessor;

        public IHttpMethodHandler GetHttpMethodHandler(string httpMethod)
        {
            var handler = _handlers.GetValueOrDefault(httpMethod, null);
            if(handler == null) {
                handler = this.CreateHttpMethodHandler(httpMethod);
                _handlers.Add(httpMethod, handler);
            }
            return handler;
        }

        private IHttpMethodHandler CreateHttpMethodHandler(string httpMethod)
        {
            switch (httpMethod)
            {
                case string s when s == HttpMethods.Get:
                    return new HttpGetHandler(_dbAccessor);
                case string s when s == HttpMethods.Post:
                    return new HttpPostHandler(_dbAccessor);
                case string s when s == HttpMethods.Put:
                    return new HttpPutHandler(_dbAccessor);
                case string s when s == HttpMethods.Patch:
                    return new HttpPatchHandler(_dbAccessor);
                case string s when s == HttpMethods.Delete:
                    return new HttpDeleteHandler(_dbAccessor);
                case string s when s == HttpMethods.Options:
                    return new HttpOptionsHandler(_dbAccessor);
                case string s when s == HttpMethods.Head:
                    return new HttpHeadHandler(_dbAccessor);
                case string s when s == HttpMethods.Connect:
                    return new HttpConnectHandler(_dbAccessor);
                case string s when s == HttpMethods.Trace:
                    return new HttpTraceHandler(_dbAccessor);
                default:
                    throw new NotSupportedException($"Unknown HTTP method: {httpMethod}");
            }
        }
    }
}
