using ToSic.Sxc.Code.Razor.Sys;
using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Services.Cache;
using ToSic.Sxc.Services.Cache.Sys;

namespace ToSic.Sxc.Code.Razor;

[PrivateApi("not yet public or final, WIP v20.00.0x, will have to create interface")]
public class RazorConfiguration(RenderSpecs renderSpecs, ILog parentLog): HelperBase(parentLog, "Rzr.Config"), IRazorConfiguration
{
    // This class is a placeholder for future Razor configuration settings.
    // It is currently empty and serves as a temporary structure for potential future use.
    // The class may be expanded with properties and methods as needed in the future.

    public string? Partial(NoParamOrder protector = default, Func<ICacheSpecs, ICacheSpecs>? cache = default)
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

    public string? PartialCache(NoParamOrder protector = default, bool useDefaults = true, int? sliding = null, string? watch = null, string? varyBy = null, string? url = null, string? model = null)
    {
        var config = new CacheConfig(
            sliding: sliding ?? (useDefaults ? DefaultSliding : null),
            watch: watch ?? (useDefaults ? DefaultWatch : null),
            varyBy: varyBy ?? (useDefaults ? DefaultVaryBy : null),
            url: url ?? (useDefaults ? DefaultUrl : null),
            model: model ?? (useDefaults ? DefaultModel : null)
        );

        Parent.CacheSpecs = config.RestoreAll(Parent.CacheSpecs);

        return null;
    }

    /// <summary>
    /// Default value for sliding cache duration in seconds (5 minutes). Used as fallback when `useDefaults` is true.
    /// </summary>
    public const int DefaultSliding = 300;

    public const string DefaultWatch = "data,folder";

    public const string DefaultVaryBy = "page,module,user";

    public const string DefaultUrl = "";

    public const string DefaultModel = "";

    [field: AllowNull, MaybeNull]
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
