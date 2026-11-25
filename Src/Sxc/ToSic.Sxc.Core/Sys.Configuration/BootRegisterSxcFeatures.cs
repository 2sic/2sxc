using ToSic.Sys.Boot;
using ToSic.Sys.Capabilities.Features;
using static ToSic.Sxc.Sys.Configuration.SxcFeatures;

namespace ToSic.Sxc.Sys.Configuration;

[ShowApiWhenReleased(ShowApiMode.Never)]
public sealed class BootRegisterSxcFeatures(FeaturesCatalog featuresCatalog)
    : ServiceBase($"{SxcLogName}.SUpReg", connect: [featuresCatalog]), IBootProcess
{

    public string NameId => Log.NameId;

    public BootPhase Phase => BootPhase.Registrations;

    public int Priority => 999;

    /// <summary>
    /// Register Sxc features during boot.
    /// </summary>
    public void Run() => featuresCatalog.Register(SxcFeatures);

    private static readonly Feature[] SxcFeatures =
    [
        // CorePlus
        RazorThrowPartial,
        RenderThrowPartialSystemAdmin,
        PermissionPrioritizeModuleContext,

        // Sentinel CSP
        ContentSecurityPolicy,
        ContentSecurityPolicyTestUrl,
        ContentSecurityPolicyEnforceTemp,

        // Sentinel New 15.04
        CdnSourcePublic,
        CdnSourceEdit,
        // Not yet available
        //CdnSourceAdmin,
        //CdnSourceDev,

        // Sentinel new v18.05
        NetworkDataEncryption,

        // Patrons Perfectionist
        ImageServiceMultiFormat, // v13
        ImageServiceMultipleSizes,
        ImageServiceSetSizes,
        ImageServiceUseFactors,

        LightSpeedOutputCache,
        LightSpeedOutputCacheAppFileChanges,
        LightSpeedOutputCachePartials, // v20
        LightSpeedOutputCacheCompression, // v21
        SmartDataCache, // v19.01

        PageShieldFloodGates, // v21.06

        RazorCacheCompiledToDisk, // v20.00-09
    ];
}