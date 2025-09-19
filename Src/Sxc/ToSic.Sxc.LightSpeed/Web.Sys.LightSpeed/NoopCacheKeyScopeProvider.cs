namespace ToSic.Sxc.Web.Sys.LightSpeed;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class NoopCacheKeyScopeProvider : ICacheKeyScopeProvider
{
    public string? BuildScopeSegment() => null;
}
