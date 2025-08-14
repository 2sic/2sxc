using ToSic.Sxc.Dnn.Razor.Sys;
using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Services.Cache;

namespace ToSic.Sxc.Custom.Hybrid;

[PrivateApi("not yet public or final, WIP v20.00.0x")]
public class RazorConfigurationTemp(RenderSpecs renderSpecs)
{
    // This class is a placeholder for future Razor configuration settings.
    // It is currently empty and serves as a temporary structure for potential future use.
    // The class may be expanded with properties and methods as needed in the future.

    public string PartCaching(Func<ICacheSpecs, ICacheSpecs> config)
    {
        if (config == null)
            return "";

        var parent = (RenderPartialSpecsWithCaching)renderSpecs.PartialSpecs;

        // On first use, enable caching since it was off at first
        if (!_cachingWasCalled)
        {
            parent.CacheSpecs = parent.CacheSpecs.Enable();
            _cachingWasCalled = true;
        }

        var updated = config(parent.CacheSpecs);
        parent.CacheSpecs = updated;
        return null;
    }

    private bool _cachingWasCalled;
}
