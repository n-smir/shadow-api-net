using Microsoft.AspNetCore.Builder;

namespace ShadowApiNet.Extensions
{
    public static class IApplicationBuilderExt
    {
        public static IApplicationBuilder UseShadowApi(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiResolverMiddleware>();
        }
    }
}
