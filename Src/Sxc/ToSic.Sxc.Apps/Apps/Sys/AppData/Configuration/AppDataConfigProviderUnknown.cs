using ToSic.Eav.LookUp.Sys.Engines;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Apps.Sys;

internal class AppDataConfigProviderUnknown(WarnUseOfUnknown<AppDataConfigProviderUnknown> _) : IAppDataConfigProvider
{
    public IAppDataConfiguration GetDataConfiguration(SxcAppBase app, AppDataConfigSpecs specs) => new AppDataConfiguration(new LookUpEngine(app.Log));
}