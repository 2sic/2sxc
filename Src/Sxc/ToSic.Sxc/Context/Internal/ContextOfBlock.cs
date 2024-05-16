using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Internal;
using ToSic.Lib.DI;
using ToSic.Sxc.Cms.Internal.Publishing;
using ToSic.Sxc.Web.Internal.PageService;

namespace ToSic.Sxc.Context.Internal;

[PrivateApi("Internal stuff, not for public use")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class ContextOfBlock: ContextOfApp, IContextOfBlock
{
    #region Constructor / DI

    // Important: this is the third inheritance, so it cannot create
    // another MyServices to inherit again, as it's not supported across three level
    // So these dependencies must be in the constructor
    public ContextOfBlock(
        IPage page, 
        IModule module,
        LazySvc<ServiceSwitcher<IPagePublishingGetSettings>> publishingResolver,
        PageServiceShared pageServiceShared,
        MyServices appServices
    ) : base(appServices, "Sxc.CtxBlk")
    {
        Page = page;
        ConnectLogs([
            Module = module,
            PageServiceShared = pageServiceShared,
            _publishingResolver = publishingResolver
        ]);
    }
    private readonly LazySvc<ServiceSwitcher<IPagePublishingGetSettings>> _publishingResolver;

    #endregion

    #region Override AppIdentity based on module information

    protected override IAppIdentity AppIdentity
    {
        get
        {
            if (base.AppIdentity != null) return base.AppIdentity;
            var l = Log.Fn<IAppIdentity>();
            var identifier = Module?.BlockIdentifier;
            if (identifier == null) return l.ReturnNull("no mod-block-id");
            AppIdentity = identifier;
            return l.Return(base.AppIdentity);
        }
    }

    #endregion

    /// <inheritdoc />
    public IPage Page { get; }

    /// <inheritdoc />
    public IModule Module { get; }

    public PageServiceShared PageServiceShared { get; }

    /// <inheritdoc />
    public BlockPublishingSettings Publishing => _publishing ??= _publishingResolver.Value.Value.SettingsOfModule(Module?.Id ?? -1);
    private BlockPublishingSettings _publishing;

    /// <inheritdoc />
    public new IContextOfSite Clone(ILog parentLog) => new ContextOfBlock(Page, Module, _publishingResolver, PageServiceShared, AppServices)
        .LinkLog(parentLog);
}