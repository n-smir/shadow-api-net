using Microsoft.AspNetCore.Http;
using ShadowApiNet.Abstractions;
using ShadowApiNet.HttpHandlers;
using System;

namespace ShadowApiNet
{
    internal class HttpMethodHandlerFactory : IHttpHandlerFactory
    {
        public IHttpMethodHandler GetHttpMethodHandler(string httpMethod)
        {
            switch (httpMethod) {
                case string s when s == HttpMethods.Get:
                    return new HttpGetHandler();
                case string s when s == HttpMethods.Post:
                    return new HttpPostHandler();
                case string s when s == HttpMethods.Put:
                    return new HttpPutHandler();
                case string s when s == HttpMethods.Patch:
                    return new HttpPatchHandler();
                case string s when s == HttpMethods.Delete:
                    return new HttpDeleteHandler();
                case string s when s == HttpMethods.Options:
                    return new HttpOptionsHandler();
                case string s when s == HttpMethods.Head:
                    return new HttpHeadHandler();
                case string s when s == HttpMethods.Connect:
                    return new HttpConnectHandler();
                case string s when s == HttpMethods.Trace:
                    return new HttpTraceHandler();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
