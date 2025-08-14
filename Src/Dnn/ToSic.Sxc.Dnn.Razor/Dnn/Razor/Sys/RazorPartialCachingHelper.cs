using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Services.Cache;
using ToSic.Sxc.Sys.Configuration;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sxc.Web.Sys.LightSpeed;

namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// Helper to manage Razor partial caching.
/// </summary>
/// <param name="normalizedPath"></param>
/// <param name="exCtx"></param>
/// <param name="featureSvc"></param>
/// <param name="parentLog"></param>
internal class RazorPartialCachingHelper(string normalizedPath, IExecutionContext exCtx, IFeaturesService featureSvc, ILog parentLog) : HelperBase(parentLog, "Rzr.Cache")
{
    /// <summary>
    /// App Id
    /// </summary>
    private int AppId => _appId ??= exCtx.GetAppId();
    private int? _appId;

    /// <summary>
    /// Underlying cache service, taken from the execution context so it knows more about the current request.
    /// </summary>
    private ICacheService CacheServiceFromExCtx => field ??= exCtx.GetService<ICacheService>();

    /// <summary>
    /// Cache specs prepared for the current partial rendering - contains the path.
    /// </summary>
    public ICacheSpecs CacheSpecs
    {
        get
        {
            if (field != null)
                return field;

            var cacheKeyString = OutputCacheKeys.PartialKey(AppId, normalizedPath);
            field = CacheServiceFromExCtx.CreateSpecs("***" + cacheKeyString); // "***" is to override normal key generation for now
            return field;
        }
    }

    /// <summary>
    /// Try to get the data from the cache.
    /// </summary>
    /// <returns></returns>
    public string? TryGetFromCache()
    {
        var l = Log.Fn<string>();
        var cached = CacheServiceFromExCtx.Get<OutputCacheItem>(CacheSpecs);
        return cached == null
            ? l.ReturnNull("not cached") :
            // If we have a cached result, return it
            l.Return(cached.Data.Html, "Returning cached result");
    }

    public bool SaveToCache(string html, ICacheSpecs partialSpecs)
    {
        var l = Log.Fn<bool>();
        if (!featureSvc.IsEnabled(SxcFeatures.LightSpeedOutputCachePartials.NameId) || !partialSpecs.IsEnabled)
            return l.ReturnFalse("no partial caching");

        l.A($"Add to cache");
        // var cacheSpecs = partialSpecs.SetSlidingExpiration(new(0, 5, 0));
        CacheServiceFromExCtx.Set(partialSpecs, new OutputCacheItem(new RenderResult { AppId = AppId, Html = html, IsPartial = true }));

        return l.ReturnTrue("Saved to cache");
    }
}
