using ToSic.Sxc.Services;

namespace ToSic.Sxc.Demo;

/// <summary>
/// Demo extensions to help in tutorials with the ToolbarService.
///
/// Not meant for production, could change at any time. 
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public static class ToolbarServiceExtensions
{
    /// <summary>
    /// Created for 14.08, used in the tutorial starting 2022-08-23
    /// </summary>
    /// <param name="toolbarService"></param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="ui"></param>
    public static void ActivateDemoMode(this IToolbarService toolbarService, 
        NoParamOrder noParamOrder = default,
        string ui = null
    )
    {
        if (toolbarService is not ToolbarService typed) return;
        typed._setDemoDefaults(ui);
    }
}