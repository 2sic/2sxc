using ToSic.Sxc.Services.OutputCache;

namespace ToSic.Sxc.Web.Sys.LightSpeed;

/// <summary>
/// LightSpeed implementation of the output-cache flush bridge used by <see cref="IOutputCacheService"/>.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class OutputCacheFlushService(LazySvc<LightSpeedExternalDependencies> externalDependencies)
    : ServiceBase(SxcLogName + ".OutCFl", connect: [externalDependencies]), IOutputCacheFlushService
{
    public int Flush(int appId, IEnumerable<string>? dependencies)
        => ExternalDependencies.Touch(appId, dependencies);

    public void FlushApp(int appId)
        => ExternalDependencies.TouchApp(appId);

    private LightSpeedExternalDependencies ExternalDependencies => externalDependencies.Value;
}
