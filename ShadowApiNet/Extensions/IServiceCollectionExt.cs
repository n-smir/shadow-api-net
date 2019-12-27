using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShadowApiNet.Interfaces;

namespace ShadowApiNet.Extensions
{
    public static class IServiceCollectionExt
    {
        private const string DefaultUriPath = "/dataapi";

        public static IServiceCollection AddShadowApi(this IServiceCollection services, DbContext context, string rootUriPath = DefaultUriPath)
        {
            return services.AddSingleton<IApiResolver>(new ApiResolver(context, rootUriPath));
        }
    }
}
