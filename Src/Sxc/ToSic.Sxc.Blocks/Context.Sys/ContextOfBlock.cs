using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sxc.Sys.Render.PageContext;

namespace ToSic.Sxc.Context.Sys;

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
    LazySvc<ServiceSwitcher<IPagePublishingGetSettings>> publishingResolver,
    IPageServiceShared pageServiceShared,
    ContextOfApp.Dependencies appServices)
    : ContextOfApp(appServices, "Sxc.CtxBlk", connect: [module, pageServiceShared, publishingResolver]), IContextOfBlock
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
    public BlockPublishingSettings Publishing => field ??= publishingResolver.Value.Value.SettingsOfModule(Module?.Id ?? -1);

    /// <inheritdoc />
    public override IContextOfSite Clone(ILog parentLog)
        => new ContextOfBlock(Page, Module, publishingResolver, PageServiceShared, AppServices)
            .LinkLog(parentLog);
}