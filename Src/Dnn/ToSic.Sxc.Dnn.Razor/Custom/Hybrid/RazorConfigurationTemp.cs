using ToSic.Sxc.Engines;
using ToSic.Sxc.Engines.Sys;

namespace ToSic.Sxc.Custom.Hybrid;

[PrivateApi("not yet public or final, WIP v20.00.0x")]
public class RazorConfigurationTemp(ViewDataWithModel viewDataAndFeedback)
{
    // This class is a placeholder for future Razor configuration settings.
    // It is currently empty and serves as a temporary structure for potential future use.
    // The class may be expanded with properties and methods as needed in the future.

    public string TestSet(bool alwaysCache)
    {
        if (viewDataAndFeedback != null)
            viewDataAndFeedback.AlwaysCache = alwaysCache;
        return null;
    }
}
