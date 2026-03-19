namespace ToSic.Sxc.Services.OutputCache;

/// <summary>
/// Fallback implementation used when LightSpeed is not registered.
/// It keeps <see cref="OutputCacheService"/> resolvable in tests or hosts without LightSpeed.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class OutputCacheFlushServiceUnknown : IOutputCacheFlushService
{
    public int Flush(int appId, IEnumerable<string>? dependencies) => 0;

    public void FlushApp(int appId) { }
}
