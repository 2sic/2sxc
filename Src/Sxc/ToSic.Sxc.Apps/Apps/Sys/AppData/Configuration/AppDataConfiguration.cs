using ToSic.Eav.LookUp.Sys.Engines;

namespace ToSic.Sxc.Apps.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppDataConfiguration(ILookUpEngine configuration, bool? showDrafts = null) : IAppDataConfiguration
{
    public bool? ShowDrafts { get; } = showDrafts;


    public ILookUpEngine Configuration { get; } = configuration;
}