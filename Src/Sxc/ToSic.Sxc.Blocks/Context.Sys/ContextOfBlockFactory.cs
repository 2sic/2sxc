using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sxc.Sys.Render.PageContext;

namespace ToSic.Sxc.Context.Sys;

// Notes 2026-10-18 2dm
// This seems to never be injected, but always passed around
// So changing things here should be fairly easy, with little side effects.

/// <remarks>
/// Important: this is the third inheritance, so it cannot create
/// another MyServices to inherit again, as it's not supported across three level
/// So these dependencies must be in the constructor
/// </remarks>
[PrivateApi("Internal stuff, not for public use")]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class ContextOfBlockFactory(LazySvc<BlockPublishingSettingsService> pagePubSettings, IPageServiceShared pageServiceShared, IContextOfAppFactory appCtxFactory)
    : ServiceBase("Sxc.CtxBlk", connect: [pageServiceShared, pagePubSettings, appCtxFactory]),
        IContextOfBlockFactory
{

    #region Factory work

    public IContextOfModule Create(ISite site, IPage page, IModule module)
    {
        var l = Log.Fn<IContextOfModule>();
        // Get app identity from module
        var identifier = module.BlockIdentifier;
        var appCtx = (ContextOfApp)appCtxFactory.Create(identifier);


        var ctx = new ContextOfModule(site, appCtx.User, page, module, appCtx.AppReaderOrNull, pageServiceShared)
        {
            InternalMachinery = appCtx.InternalMachinery,
            SettingsStackSvc = appCtx.SettingsStackSvc,
            PagePubSettings = pagePubSettings,
        };

        return l.Return(ctx);
    }

    public IContextOfModule Update(IContextOfModule contextOfModule, IAppIdentity appIdentity)
    {
        var l = Log.Fn<IContextOfModule>();

        var appCtx = (ContextOfApp)appCtxFactory.Create(appIdentity);
        var ctx = new ContextOfModule(contextOfModule.Site, contextOfModule.User, contextOfModule.Page, contextOfModule.Module, appCtx.AppReaderOrNull, contextOfModule.PageServiceShared)
        {
            InternalMachinery = appCtx.InternalMachinery,
            SettingsStackSvc = appCtx.SettingsStackSvc,
            PagePubSettings = pagePubSettings,
        };

        return l.Return(ctx);
    }

    #endregion

}