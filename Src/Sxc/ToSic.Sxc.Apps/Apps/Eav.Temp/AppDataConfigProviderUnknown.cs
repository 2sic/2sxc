using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.LookUp.Engines;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Eav.Apps.Internal;

internal class AppDataConfigProviderUnknown(WarnUseOfUnknown<AppDataConfigProviderUnknown> _) : IAppDataConfigProvider
{
    public IAppDataConfiguration GetDataConfiguration(EavApp app, AppDataConfigSpecs specs) => new AppDataConfiguration(new LookUpEngine(app.Log));
}