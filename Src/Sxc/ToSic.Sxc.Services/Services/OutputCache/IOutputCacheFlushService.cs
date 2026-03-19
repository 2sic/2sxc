namespace ToSic.Sxc.Services.OutputCache;

/// <summary>
/// Internal bridge used by <see cref="IOutputCacheService"/> to touch LightSpeed dependency markers
/// without making the Services assembly depend directly on the LightSpeed assembly.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IOutputCacheFlushService
{
    int Flush(int appId, IEnumerable<string>? dependencies);

    void FlushApp(int appId);
}
