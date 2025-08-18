using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Services.Cache;
using ToSic.Sxc.Services.Cache.Sys;
using ToSic.Sxc.Web.Sys.LightSpeed;

namespace ToSic.Sxc.Custom.Hybrid;

[PrivateApi("not yet public or final, WIP v20.00.0x, will have to create interface")]
public class RazorConfiguration(RenderSpecs renderSpecs, ILog parentLog): HelperBase(parentLog, "Rzr.Config"), IRazorConfiguration
{
    // This class is a placeholder for future Razor configuration settings.
    // It is currently empty and serves as a temporary structure for potential future use.
    // The class may be expanded with properties and methods as needed in the future.

    public string Partial(NoParamOrder protector = default, Func<ICacheSpecs, ICacheSpecs> cache = default)
    {
        if (cache != null)
            try
            {
                var updated = cache(Parent.CacheSpecs);
                Parent.CacheSpecs = updated;
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
            }

        // Here future options will be added if needed.

        // Return nothing, so that Razor doesn't output anything.
        return null;
    }

    public string PartialCache(NoParamOrder protector = default, int? sliding = null, string watch = null, string varyBy = null, string url = null, string model = null)
    {
        var config = new CacheConfig(protector, sliding, watch, varyBy, url, model);

        Parent.CacheSpecs = config.RestoreAll(Parent.CacheSpecs);

        return null;
    }

    private RenderPartialSpecsWithCaching Parent
    {
        get
        {
            if (field is not null)
                return field;
            field = (RenderPartialSpecsWithCaching)renderSpecs.PartialSpecs;
            // On first use, enable caching since it was off at first
            field.CacheSpecs = field.CacheSpecs.Enable();
            return field;
        }
    }
}
