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
internal class ContextOfBlock(
    IPage page,
    IModule module,
    LazySvc<BlockPublishingSettingsService> pagePubSettings,
    IPageServiceShared pageServiceShared,
    ContextOfApp.Dependencies appServices)
    : ContextOfApp(appServices, "Sxc.CtxBlk", connect: [module, pageServiceShared, pagePubSettings]), IContextOfBlock, IContextOfSiteOrMoreCloneable
{

    #region Override AppIdentity based on module information

    protected override IAppIdentity? AppIdentity
    {
        get
        {
            if (base.AppIdentity != null)
                return base.AppIdentity;
            var l = Log.Fn<IAppIdentity?>();
            var identifier = Module?.BlockIdentifier;
            if (identifier == null)
                return l.ReturnNull("no mod-block-id");
            AppIdentity = identifier;
            return l.Return(base.AppIdentity);
        }
    }

    #endregion

    /// <inheritdoc />
    public IPage Page { get; } = page;

    /// <inheritdoc />
    public IModule Module { get; } = module;

    public IPageServiceShared PageServiceShared { get; } = pageServiceShared;

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public BlockPublishingSettings Publishing => field
        ??= pagePubSettings.Value
            .Handle(new(), new(new() { ModuleId = Module?.Id ?? -1 }))
            .GetAwaiter()
            .GetResult()
            .Data;


    /// <inheritdoc />
    /// <remarks>
    /// Note: this allows a clone pretending to be a SiteContext
    /// but, it can also preserve the block information, for deeper security checks
    /// Code should be improved so the intent is clearer
    /// </remarks>
    IContextOfSite IContextOfSiteOrMoreCloneable.CloneFullContext(ILog parentLog, ISite site)
        => new ContextOfBlock(Page, Module, pagePubSettings, PageServiceShared, AppServices with { SiteServices = AppServices.SiteServices with { Site = site } })
            .LinkLog(parentLog);

    public IContextOfBlock Clone(ILog parentLog, IAppIdentity appIdentity)
    {
        // #ReworkingResetApp - this one could use a generator
        var newCtx = new ContextOfBlock(Page, Module, pagePubSettings, PageServiceShared, AppServices)
            .LinkLog(parentLog);
        newCtx.ResetApp(appIdentity);
        return newCtx;
    }

    public IContextOfBlock ResetWip(ILog parentLog, ISite site, IModule module)
    {
        // #ReworkingResetApp - this one could use a generator
        var newCtx = new ContextOfBlock(Page, module, pagePubSettings, PageServiceShared, AppServices with {SiteServices = AppServices.SiteServices with { Site = site }})
            .LinkLog(parentLog);
        //newCtx.ResetApp(appIdentity);
        return newCtx;
    }
}