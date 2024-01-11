using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;

namespace ToSic.Sxc.Dnn.Run;
// Internal note: this is being used publicly, so as we phase it out, make sure the Namespace etc. doesn't change!

/// <summary>
/// Provides information about the current context within DNN.
/// This only applies to 2sxc running inside DNN, not inside another platform.
///
/// This object is the root `Dnn` object, used mainly in older Razor.
/// If possible, try to use `CmsContext` instead.
/// </summary>
/// <remarks>
/// It is currently on `ToSic.Sxc.Run.IDnnContext` but we plan to move it elsewhere
/// </remarks>
[PublicApi("This is DNN only, if possibly, try to use the hybrid CmsContext (v14) / MyContext (v16+) instead.")]
public interface IDnnContext
{
    /// <summary>
    /// The current DNN ModuleInfo - Dnn specific and with all the specials of the Dnn APIs.
    ///
    /// If possible, try to use [CmsContext.Module](xref:ToSic.Sxc.Context.ICmsModule) instead. 
    /// </summary>
    ModuleInfo Module { get; }

    /// <summary>
    /// The current DNN TabInfo (page). 
    /// We also don't like the name Tab, but that's the DNN convention.
    ///
    /// If possible, try to use [CmsContext.Page](xref:ToSic.Sxc.Context.ICmsPage) instead. 
    /// </summary>
    TabInfo Tab { get; }

    /// <summary>
    /// The current DNN Portal Settings.
    ///
    /// If possible, try to use [CmsContext.Site](xref:ToSic.Sxc.Context.ICmsSite) instead. 
    /// </summary>
    PortalSettings Portal { get; }

    /// <summary>
    /// The current DNN User.
    ///
    /// If possible, try to use [CmsContext.User](xref:ToSic.Sxc.Context.ICmsUser) instead.
    /// </summary>
    UserInfo User { get; }
}