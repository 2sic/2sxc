using ToSic.Eav.LookUp.Sys.Engines;

namespace ToSic.Sxc.Apps.Sys;
public static class AppExtensionsInternal
{
    public static ILookUpEngine? TryGetAppLookUpEngineOrNull(this IApp? app)
        => (app as SxcAppBase)?.AppDataConfig.LookUpEngine;
}
