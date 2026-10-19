using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sys.Users;

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
internal class ContextOfBlock(LazySvc<BlockPublishingSettingsService> pagePubSettings, IPageServiceShared pageServiceShared, IContextOfAppFactory appCtxFactory)
    : ServiceBase("Sxc.CtxBlk", connect: [pageServiceShared, pagePubSettings, appCtxFactory]), /*IContextOfBlock,*/ IContextOfBlockFactory
{

    #region Override AppIdentity based on module information

    //protected override IAppIdentity? AppIdentity
    //{
    //    get
    //    {
    //        if (base.AppIdentity != null)
    //            return base.AppIdentity;
    //        var l = Log.Fn<IAppIdentity?>();
    //        var identifier = Module?.BlockIdentifier;
    //        if (identifier == null)
    //            return l.ReturnNull("no mod-block-id");
    //        AppIdentity = identifier;
    //        return l.Return(base.AppIdentity);
    //    }
    //}

    #endregion

    ///// <inheritdoc />
    //public IPage Page { get; } = page;

    ///// <inheritdoc />
    //public IModule Module { get; init; } = null!;

    //public IPageServiceShared PageServiceShared { get; } = pageServiceShared;

    ///// <inheritdoc />
    //[field: AllowNull, MaybeNull]
    //public BlockPublishingSettings Publishing => field
    //    ??= pagePubSettings.Value
    //        .Handle(new(), new(new() { ModuleId = Module?.Id ?? -1 }))
    //        .GetAwaiter()
    //        .GetResult()
    //        .Data;

    
    //public override IContextOfSite WithSwappedSite(ISite site)
    //{
    //    // If it's a block context, we want to preserve the block information, so we return a ContextOfBlock
    //    return new ContextOfBlock(Page, pagePubSettings, PageServiceShared, Services with { Site = site })
    //        {
    //            Module = Module,
    //        };
    //}


    #region Factory work

    public IContextOfBlock Create(ISite site, IPage page, IModule module)
    {
        var l = Log.Fn<IContextOfBlock>();
        // Get app identity from module
        var identifier = module.BlockIdentifier;
        var appCtx = (ContextOfAppRecord)appCtxFactory.Create(identifier);


        var ctx = new ContextOfBlockRecord(site, appCtx.User, page, module, appCtx.AppReaderOrNull, pageServiceShared)
        {
            InternalMachinery = appCtx.InternalMachinery,
            SettingsStackSvc = appCtx.SettingsStackSvc,
            PagePubSettings = pagePubSettings,
        };

        return l.Return(ctx);

        //return new ContextOfBlock(page, pagePubSettings, PageServiceShared, Services with { Site = site })
        //{
        //    Module = module,
        //};
    }

    public IContextOfBlock Update(IContextOfBlock contextOfBlock, IAppIdentity appIdentity)
    {
        var l = Log.Fn<IContextOfBlock>();

        var appCtx = (ContextOfAppRecord)appCtxFactory.Create(appIdentity);
        var ctx = new ContextOfBlockRecord(contextOfBlock.Site, contextOfBlock.User, contextOfBlock.Page, contextOfBlock.Module, appCtx.AppReaderOrNull, contextOfBlock.PageServiceShared)
        {
            InternalMachinery = appCtx.InternalMachinery,
            SettingsStackSvc = appCtx.SettingsStackSvc,
            PagePubSettings = pagePubSettings,
        };

        return l.Return(ctx);


        ////return contextOfBlock.Clone(appIdentity);
        //// #ReworkingResetApp - this one could use a generator
        //var newCtx = new ContextOfBlock(contextOfBlock.Page, pagePubSettings, contextOfBlock.PageServiceShared, Services)
        //{
        //    Module = contextOfBlock.Module,
        //};
        //newCtx.ResetApp(appIdentity);
        //return newCtx;
    }

    #endregion

}

internal record ContextOfBlockRecord(
    ISite Site,
    IUser User,
    IPage Page,
    IModule Module,
    IAppReader? AppReaderOrNull,
    IPageServiceShared PageServiceShared
) : ContextOfAppRecord(Site, User, AppReaderOrNull), IContextOfBlock
{
    internal required LazySvc<BlockPublishingSettingsService> PagePubSettings { get; init; }

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public BlockPublishingSettings Publishing => field
        ??= PagePubSettings.Value
            .Handle(new(), new(new() { ModuleId = Module?.Id ?? -1 }))
            .GetAwaiter()
            .GetResult()
            .Data;


    IContextOfSite IContextOfSite.WithSwappedSite(ISite site) => this with { Site = site };
}