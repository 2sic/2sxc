using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Services.Cache;
using ToSic.Sxc.Web.Sys.LightSpeed;

namespace ToSic.Sxc.Custom.Hybrid;

[PrivateApi("not yet public or final, WIP v20.00.0x, will have to create interface")]
public class RazorConfiguration(RenderSpecs renderSpecs, ILog parentLog): HelperBase(parentLog, "Rzr.Config")
{
    // This class is a placeholder for future Razor configuration settings.
    // It is currently empty and serves as a temporary structure for potential future use.
    // The class may be expanded with properties and methods as needed in the future.

    public string OutputPartial(NoParamOrder protector = default, Func<ICacheSpecs, ICacheSpecs> cache = default)
    {
        if (cache != null)
            RunOutputPartialCache(cache);

        // Here future options will be added if needed.

        // Return nothing, so that Razor doesn't output anything.
        return null;
    }

    private void RunOutputPartialCache(Func<ICacheSpecs, ICacheSpecs> cache)
    {
        try
        {
            var parent = (RenderPartialSpecsWithCaching)renderSpecs.PartialSpecs;

            // On first use, enable caching since it was off at first
            if (!_cachingWasCalled)
            {
                parent.CacheSpecs = parent.CacheSpecs.Enable();
                _cachingWasCalled = true;
            }

            var updated = cache(parent.CacheSpecs);
            parent.CacheSpecs = updated;
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
        }
    }

    private bool _cachingWasCalled;

}
