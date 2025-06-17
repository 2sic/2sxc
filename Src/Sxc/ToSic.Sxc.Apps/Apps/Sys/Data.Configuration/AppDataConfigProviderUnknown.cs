using ToSic.Lib.LookUp.Engines;
using ToSic.Sxc.Apps.Sys;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Eav.Apps.Internal;

internal class AppDataConfigProviderUnknown(WarnUseOfUnknown<AppDataConfigProviderUnknown> _) : IAppDataConfigProvider
{
    public IAppDataConfiguration GetDataConfiguration(SxcAppBase app, AppDataConfigSpecs specs) => new AppDataConfiguration(new LookUpEngine(app.Log));
}