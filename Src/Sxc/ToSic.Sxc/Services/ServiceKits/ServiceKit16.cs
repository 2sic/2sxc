using Connect.Koi;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Services;

/// <summary>
/// Default ServiceKit for 2sxc v14.
/// </summary>
/// <remarks>
/// * History: Added v14.04
/// </remarks>
[PublicApi]
[method: PrivateApi("Public constructor for DI")]
public class ServiceKit16() : ServiceKit("Sxc.Kit16")
{
    /// <inheritdoc cref="ServiceKit14.Adam"/>
    public IAdamService Adam => _adam.Get(GetKitService<IAdamService>);
    private readonly GetOnce<IAdamService> _adam = new();

    /// <summary>
    /// The CMS Service - WIP
    /// </summary>
    [PrivateApi("Not yet for public use, as API is not yet public")]
    internal ICmsService Cms => _cms.Get(GetKitService<ICmsService>);
    private readonly GetOnce<ICmsService> _cms = new();

    /// <inheritdoc cref="ServiceKit14.Convert"/>
    public IConvertService16 Convert => _convert.Get(GetKitService<IConvertService16>);
    private readonly GetOnce<IConvertService16> _convert = new();

    /// <inheritdoc cref="ServiceKit14.Css"/>
    public ICss Css => _css.Get(GetKitService<ICss>);
    private readonly GetOnce<ICss> _css = new();


    /// <inheritdoc cref="ServiceKit14.Data"/>
    public IDataService Data => _data.Get(GetKitService<IDataService>);
    private readonly GetOnce<IDataService> _data = new();

    /// <inheritdoc cref="ServiceKit14.Edit"/>
    public IEditService Edit => _edit.Get(GetKitService<IEditService>);
    private readonly GetOnce<IEditService> _edit = new();


    /// <inheritdoc cref="ServiceKit14.Feature"/>
    public IFeaturesService Feature => _features.Get(GetKitService<IFeaturesService>);
    private readonly GetOnce<IFeaturesService> _features = new();

    /// <inheritdoc cref="ServiceKit14.HtmlTags"/>
    public IHtmlTagsService HtmlTags => _ht.Get(GetKitService<IHtmlTagsService>);
    private readonly GetOnce<IHtmlTagsService> _ht = new();

    /// <inheritdoc cref="ServiceKit14.Image"/>
    public IImageService Image => _image.Get(GetKitService<IImageService>);
    private readonly GetOnce<IImageService> _image = new();


    /// <inheritdoc cref="ServiceKit14.Json"/>
    public IJsonService Json => _json.Get(GetKitService<IJsonService>);
    private readonly GetOnce<IJsonService> _json = new();


    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => _link.Get(GetKitService<ILinkService>);
    private readonly GetOnce<ILinkService> _link = new();

    /// <inheritdoc cref="ServiceKit14.SystemLog"/>
    public ISystemLogService SystemLog => _sysLog.Get(GetKitService<ISystemLogService>);
    private readonly GetOnce<ISystemLogService> _sysLog = new();

    // Removed for v16
    //public new ISystemLogService Log => SystemLog;


    /// <inheritdoc cref="ServiceKit14.Mail"/>
    public IMailService Mail => _mail.Get(GetKitService<IMailService>);
    private readonly GetOnce<IMailService> _mail = new();


    /// <inheritdoc cref="ServiceKit14.Page"/>
    public IPageService Page => _page.Get(GetKitService<IPageService>);
    private readonly GetOnce<IPageService> _page = new();


    /// <inheritdoc cref="ServiceKit14.Render"/>
    public IRenderService Render => _render.Get(GetKitService<IRenderService>);
    private readonly GetOnce<IRenderService> _render = new();

    /// <inheritdoc cref="ServiceKit14.SecureData"/>
    public ISecureDataService SecureData => _secureData.Get(GetKitService<ISecureDataService>);
    private readonly GetOnce<ISecureDataService> _secureData = new();

    /// <inheritdoc cref="ServiceKit14.Scrub"/>
    public IScrub Scrub => _scrub.Get(GetKitService<IScrub>);
    private readonly GetOnce<IScrub> _scrub = new();


    /// <inheritdoc cref="ServiceKit14.Toolbar"/>
    public IToolbarService Toolbar => _toolbar.Get(GetKitService<IToolbarService>);
    private readonly GetOnce<IToolbarService> _toolbar = new();

    /// <inheritdoc cref="ServiceKit14.User"/>
    [PrivateApi("Experimental in v15.03")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public IUserService User => _users.Get(GetKitService<IUserService>);
    private readonly GetOnce<IUserService> _users = new();

    /// <summary>
    /// Key service.
    /// Rarely used, as the RazorTyped has a UniqueKey property which comes from this service.
    /// You only need this service, if you need to create combined keys (like with an entity)
    /// </summary>
    /// <remarks>
    /// * New in v16.04
    /// </remarks>
    public IKeyService Key => _keys ??= new KeyService();
    private IKeyService _keys;

    /// <summary>
    /// Templates service, which can parse strings containing placeholders.
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Still Beta in v17.08")]
    public ITemplateService Template => _templates ??= GetKitService<ITemplateService>();
    private ITemplateService _templates;

    [InternalApi_DoNotUse_MayChangeWithoutNotice("Still Beta in v17.09")]
    public ICacheService Cache => _cache ??= GetKitService<ICacheService>();
    private ICacheService _cache;

}