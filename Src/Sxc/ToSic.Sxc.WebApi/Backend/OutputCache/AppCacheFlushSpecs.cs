namespace ToSic.Sxc.Backend.OutputCache;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppCacheFlushSpecs
{
    // Null or empty means app-wide LightSpeed invalidation; otherwise only the named dependencies are touched.
    public string[]? Dependencies { get; set; }
}
