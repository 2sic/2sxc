using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Dnn.Context;

internal class DnnContext : IDnnContext, INeedsExecutionContext
{
    /// <summary>
    /// Build DNN Helper
    /// Note that the context can be null, in which case it will have no module context, and default to the current portal
    /// </summary>
    public void ConnectToRoot(IExecutionContext exCtx)
    {
        var moduleContext = exCtx.GetState<IContextOfBlock>()?.Module;
        Module = (moduleContext as Module<ModuleInfo>)?.GetContents();
        // note: this may be a bug, I assume it should be Module.OwnerPortalId
        Portal = PortalSettings.Current ?? 
                 (Module != null ? new PortalSettings(Module.PortalID): null);
    }

    public ModuleInfo Module { get; private set; }

    public TabInfo Tab => Portal?.ActiveTab;

    public PortalSettings Portal { get; private set; }

    public UserInfo User => Portal.UserInfo;
}