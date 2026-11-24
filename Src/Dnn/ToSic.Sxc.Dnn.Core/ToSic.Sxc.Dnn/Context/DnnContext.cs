using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using ToSic.Sxc.Code.Context.Sys;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Wrappers;

namespace ToSic.Sxc.Dnn.Context;

internal class DnnContext(Ctx<ICodeContext> exCtx) : IDnnContext
{
    /// <summary>
    /// Build DNN Helper
    /// Note that the context can be null, in which case it will have no module context, and default to the current portal
    /// </summary>
    private ICodeContext? ExCtx => exCtx.ValueOptional;

    public ModuleInfo Module => _moduleLazy.Get(() => (ExCtx?.Block?.Context?.Module as IWrapper<ModuleInfo>)?.GetContents());
    private readonly LazyGet<ModuleInfo?> _moduleLazy = new();

    public TabInfo Tab => Portal?.ActiveTab;

    public PortalSettings Portal => _portalLazy.Get(() => PortalSettings.Current ??
                                    (Module != null ? new PortalSettings(Module.PortalID) : null));
    private readonly LazyGet<PortalSettings?> _portalLazy = new();

    public UserInfo User => Portal.UserInfo;
}
