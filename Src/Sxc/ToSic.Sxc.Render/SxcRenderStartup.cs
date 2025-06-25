using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Render.Sys.JsContext;
using ToSic.Sxc.Render.Sys.RenderBlock;
using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sxc.Sys.Render.PageFeatures;
using ToSic.Sxc.Web.Sys.ContentSecurityPolicy;
using ToSic.Sxc.Web.Sys.PageFeatures;
using ToSic.Sxc.Web.Sys.PageService;
using ToSic.Sxc.Web.Sys.PageServiceShared;

namespace ToSic.Sxc;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcRenderStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcRender(this IServiceCollection services)
    {
        services.TryAddTransient<Services.IRenderService, RenderService>();  // new 12.05
        services.TryAddTransient<RenderService.MyServices>();
        services.TryAddTransient<SimpleRenderer>();
        services.TryAddTransient<InTextContentBlockRenderer>();
        
        // #RemoveBlocksIRenderService
//#if NETFRAMEWORK
//#pragma warning disable CS0618
//        services.TryAddTransient<Blocks.IRenderService, RenderService>();  // Obsolete, but keep for the few apps we already released in v12
//#pragma warning restore CS0618
//#endif
        services.TryAddTransient<IBlockBuilder, BlockBuilder>();
        services.TryAddTransient<BlockBuilder.MyServices>();

        services.TryAddTransient<IRenderingHelper, RenderingHelper>();

        // JS UI Context for render
        services.TryAddTransient<JsContextAll>();
        services.TryAddTransient<JsContextLanguage>();
        services.TryAddScoped<JsApiCacheService>(); // v16.01


        // Content Security Policies (CSP)
        services.TryAddTransient<IContentSecurityPolicyService, ContentSecurityPolicyService>();
        services.TryAddTransient<CspOfApp>();   // must be transient
        services.TryAddScoped<CspOfModule>();   // important: must be scoped!
        services.TryAddTransient<CspOfPage>();
        services.TryAddTransient<CspParameterFinalizer>();


        // 2022-02-07 2dm experimental
        // The PageServiceShared must always be generated from the PageScope
        // I previously thought the PageServiceShared must be scoped at page level, but this is wrong
        // Reason is that it seems to collect specs per module, and then actually only flushes it
        // Because it shouldn't remain in the list for the second module
        // So it actually looks like it's very module-scoped already, but had workarounds for it.
        // So I think it really doesn't need to be have workarounds for it
        services.TryAddScoped<IPageServiceShared, PageServiceShared>();
        services.TryAddTransient<PageChangeSummary>();

        // Page Features
        services.TryAddTransient<IPageFeatures, PageFeatures>();
        services.TryAddTransient<IPageFeaturesManager, PageFeaturesManager>();
        services.TryAddSingleton<PageFeaturesCatalog>();


        services.AddSxcRenderFallback();

        return services;
    }

    public static IServiceCollection AddSxcRenderFallback(this IServiceCollection services)
    {
        // v16
        services.TryAddScoped<IJsApiService, JsApiServiceUnknown>();

        return services;
    }

}