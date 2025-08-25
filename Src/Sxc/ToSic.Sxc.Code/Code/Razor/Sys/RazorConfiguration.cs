using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Services.Cache;
using ToSic.Sxc.Services.Cache.Sys;
using ToSic.Sxc.Services.Cache.Sys.CacheKey;

namespace ToSic.Sxc.Code.Razor.Sys;

[PrivateApi("not yet public or final, WIP v20.00.0x, will have to create interface")]
public class RazorConfiguration(RenderSpecs renderSpecs, ILog parentLog): HelperBase(parentLog, "Rzr.Config"), IRazorConfiguration
{
    // This class is a placeholder for future Razor configuration settings.
    // It is currently empty and serves as a temporary structure for potential future use.
    // The class may be expanded with properties and methods as needed in the future.

    public string? PartialCache(NoParamOrder protector = default,
        int? sliding = null, // legacy name
        int? seconds = null,
        string? watch = null,
        string? varyBy = null,
        string? url = null,
        string? model = null,
        Func<ICacheSpecs, ICacheSpecs>? tweak = default)
    {
        if (Parent == null)
            return null;

        // temp!
        seconds ??= sliding;
        var l = Log.Fn<string?>($"{nameof(seconds)}: '{seconds}', {nameof(watch)}: '{watch}', {nameof(varyBy)}: '{varyBy}', {nameof(url)}: '{url}', {nameof(model)}: '{model}'");
        if (seconds != null || watch != null || varyBy != null || url != null || model != null)
        {
            l.A("Set aspects using values");
            var config = new CacheKeyConfig(seconds: seconds, varyBy: varyBy, url: url, model: model);
            var writeConfig = new CacheWriteConfig(watch: watch);
            Parent.CacheSpecs = Parent.CacheSpecs.RestoreAll(config, writeConfig);
        }

        if (tweak != null)
            try
            {
                l.A("Set aspects using function");
                Parent.CacheSpecs = tweak(Parent.CacheSpecs);
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
            }

        return l.ReturnNull();
    }

    private RenderPartialSpecsWithCaching? Parent
    {
        get
        {
            if (field is not null)
                return field;

            // paranoid, on main entry razor it doesn't exist ATM 2025-08-19
            if (renderSpecs?.PartialSpecs is not RenderPartialSpecsWithCaching typed)
                return null;

            // On first use, enable caching since it was off at first
            typed.CacheSpecs = typed.CacheSpecs.Enable();
            return field = typed;
        }
    }
}
