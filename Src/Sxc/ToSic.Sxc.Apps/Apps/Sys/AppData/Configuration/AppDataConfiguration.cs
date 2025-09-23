using ToSic.Eav.LookUp.Sys.Engines;

namespace ToSic.Sxc.Apps.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record AppDataConfiguration(
    ILookUpEngine LookUpEngine,
    bool? ShowDrafts = null
) : IAppDataConfiguration;