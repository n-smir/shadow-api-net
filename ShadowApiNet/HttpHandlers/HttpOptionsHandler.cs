﻿using Microsoft.AspNetCore.Http;
using ShadowApiNet.Interfaces;
using System;
using System.Threading.Tasks;

namespace ShadowApiNet.HttpHandlers
{
    internal class HttpOptionsHandler : IHttpMethodHandler
    {
        public Task<HttpContext> Handle(HttpContext httpContext)
        {
            throw new NotImplementedException();
        }
    }
}
