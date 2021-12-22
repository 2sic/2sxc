using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Dnn.Run
{
    /// <summary>
    /// Provides information about the current context within DNN.
    /// This only applies to 2sxc running inside DNN, not inside another platform. 
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public interface IDnnContext
    {
        /// <summary>
        /// The current DNN ModuleInfo.
        /// </summary>
        ModuleInfo Module { get; }

        /// <summary>
        /// The current DNN TabInfo (page).
        /// We also don't like the name Tab, but that's the DNN convention. 
        /// </summary>
        TabInfo Tab { get; }

        /// <summary>
        /// The current DNN Portal Settings
        /// </summary>
        PortalSettings Portal { get; }

        /// <summary>
        /// The current DNN User.
        /// </summary>
        UserInfo User { get; }
    }
}