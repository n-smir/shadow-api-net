using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShadowApiNet.Abstractions;

namespace ShadowApiNet.Extensions
{
    public static class IServiceCollectionExt
    {
        private const string DefaultUriPath = "/dataapi";

        public static IServiceCollection AddShadowApi(this IServiceCollection services, DbContext context, string rootUriPath = DefaultUriPath)
        {
            services.AddSingleton<IHttpHandlerFactory, HttpMethodHandlerFactory>();
            return services.AddSingleton<IApiResolver, ApiResolver>(service => 
            {
                return new ApiResolver(context, service.GetService<IHttpHandlerFactory>(), rootUriPath);
            });
        }
    }
}
