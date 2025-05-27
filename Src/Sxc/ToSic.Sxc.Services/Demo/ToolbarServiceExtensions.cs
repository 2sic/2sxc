using ToSic.Sxc.Services;

namespace ToSic.Sxc.Demo;

/// <summary>
/// Demo extensions to help in tutorials with the ToolbarService.
///
/// Not meant for production, could change at any time. 
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public static class ToolbarServiceExtensions
{
    /// <summary>
    /// Internal API for the Tutorials. Sets UI settings - mainly "show=always" - to better demonstrate what the toolbar does.
    /// </summary>
    /// <param name="toolbarService"></param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="ui"></param>
    /// <remarks>
    /// Created for 14.08, used in the tutorial starting 2022-08-23.
    /// As of 2025-05-26 it's still used extensively, so we must preserve it as of now.
    /// </remarks>
    public static void ActivateDemoMode(this IToolbarService toolbarService, 
        NoParamOrder noParamOrder = default,
        string ui = null
    )
    {
        if (toolbarService is not ToolbarService typed)
            return;
        typed._setDemoDefaults(ui);
    }
}