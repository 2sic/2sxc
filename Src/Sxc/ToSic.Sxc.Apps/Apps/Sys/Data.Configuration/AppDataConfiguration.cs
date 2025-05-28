using ToSic.Lib.LookUp.Engines;

namespace ToSic.Eav.Apps.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppDataConfiguration(ILookUpEngine configuration, bool? showDrafts = null) : IAppDataConfiguration
{
    public bool? ShowDrafts { get; } = showDrafts;


    public ILookUpEngine Configuration { get; } = configuration;
}