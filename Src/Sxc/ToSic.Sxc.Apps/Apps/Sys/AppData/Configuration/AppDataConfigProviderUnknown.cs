using ToSic.Eav.LookUp.Sys.Engines;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Apps.Sys;

internal class AppDataConfigProviderUnknown(WarnUseOfUnknown<AppDataConfigProviderUnknown> _) : IAppDataConfigProvider
{
    public IAppDataConfiguration GetDataConfiguration(IAppForCode appForLookup, AppDataConfigSpecs specs)
        => new AppDataConfiguration(new LookUpEngine((appForLookup as IHasLog)?.Log));
}