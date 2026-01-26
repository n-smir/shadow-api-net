using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShadowApiNet.Abstractions;
using ShadowApiNet.Utils;

namespace ShadowApiNet.Extensions
{
    public static class IServiceCollectionExt
    {
        private const string DefaultUriPath = "dataapi";

        public static IServiceCollection AddShadowApi(this IServiceCollection services, DbContext context, string rootUriPath = DefaultUriPath)
        {
            services.AddSingleton(service =>
            {
                return new DbAccessor(context);
            });
            services.AddSingleton<IHttpHandlerFactory, HttpMethodHandlerFactory>(service =>
            {
                return new HttpMethodHandlerFactory(service.GetService<DbAccessor>());
            });
            services.AddSingleton<IApiResolver, ApiResolver>(service =>
            {
                return new ApiResolver(service.GetService<IHttpHandlerFactory>(), rootUriPath);
            });

            return services;
        }
    }
}
