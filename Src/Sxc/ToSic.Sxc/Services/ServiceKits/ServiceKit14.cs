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
public class ServiceKit14() : ServiceKit("Sxc.Kit14")
{
    /// <summary>
    /// The ADAM Service, used to retrieve files and maybe more. 
    /// </summary>
    public IAdamService Adam => _adam.Get(GetKitService<IAdamService>);
    private readonly GetOnce<IAdamService> _adam = new();

    /// <summary>
    /// The CMS Service - WIP
    /// </summary>
    [PrivateApi("Not yet for public use, as API is not yet public")]
    internal ICmsService Cms => _cms.Get(GetKitService<ICmsService>);
    private readonly GetOnce<ICmsService> _cms = new();

    /// <summary>
    /// The Convert Service, used to convert any kind of data type to another data type
    /// </summary>
    public IConvertService Convert => _convert.Get(GetKitService<IConvertService>);
    private readonly GetOnce<IConvertService> _convert = new();

    /// <summary>
    /// The Koi CSS Service, used to detect the current CSS framework and other features.
    /// See [ICss](xref:Connect.Koi.ICss)
    /// </summary>
    public ICss Css => _css.Get(GetKitService<ICss>);
    private readonly GetOnce<ICss> _css = new();


    /// <summary>
    /// The Data service to get DataSources and similar.
    /// </summary>
    /// <remarks>
    /// * added in v15.06
    /// </remarks>
    public IDataService Data => _data.Get(GetKitService<IDataService>);
    private readonly GetOnce<IDataService> _data = new();

    /// <summary>
    /// The Edit service, same as the main Edit service
    /// </summary>
    // Important: must share the Edit from the _DynCodeRoot for scenarios where Enable was set manually
    public IEditService Edit => _edit.Get(GetKitService<IEditService>);
    private readonly GetOnce<IEditService> _edit = new();


    /// <summary>
    /// The Features service, used to check if features are enabled
    /// </summary>
    public IFeaturesService Feature => _features.Get(GetKitService<IFeaturesService>);
    private readonly GetOnce<IFeaturesService> _features = new();

    /// <summary>
    /// The Razor Blade 4 HtmlTags service, to fluidly create Tags.
    /// See [](xref:ToSic.Razor.Blade.IHtmlTagsService).
    ///
    /// > [!IMPORTANT]
    /// > This is _similar but different_ to the [Razor.Blade.Tag](https://razor-blade.net/api/ToSic.Razor.Blade.Tag.html).
    /// > The [](xref:ToSic.Razor.Blade.IHtmlTag) objects returned here are _immutable_.
    /// > This means that chained commands like `...HtmlTags.Div().Id(...).Class(...)`
    /// > all return new objects and don't modify the previous one.
    /// >
    /// > The older `Tag` helper created mutable objects where chaining always modified the original and returned it again.
    /// </summary>
    /// <remarks>Added in v15</remarks>
    public IHtmlTagsService HtmlTags => _ht.Get(GetKitService<IHtmlTagsService>);
    private readonly GetOnce<IHtmlTagsService> _ht = new();

    /// <summary>
    /// The Images service, used to create `img` and `picture` tags
    /// </summary>
    public IImageService Image => _image.Get(GetKitService<IImageService>);
    private readonly GetOnce<IImageService> _image = new();


    /// <summary>
    /// The JSON service, used to convert data to-and-from JSON
    /// </summary>
    public IJsonService Json => _json.Get(GetKitService<IJsonService>);
    private readonly GetOnce<IJsonService> _json = new();


    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => _link.Get(GetKitService<ILinkService>);
    private readonly GetOnce<ILinkService> _link = new();

    /// <summary>
    /// The System Log service, used to add log messages to the system (Dnn/Oqtane)
    /// </summary>
    public ISystemLogService SystemLog => _sysLog.Get(GetKitService<ISystemLogService>);
    private readonly GetOnce<ISystemLogService> _sysLog = new();

    /// <summary>
    /// Note that this was used in Mobius / Events in a few releases, so we can't just change it.
    /// If we create a Kit15, this should be removed
    /// </summary>
    [PrivateApi("was the official name before v15.06, probably never used publicly, but should stay in for a while")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public new ISystemLogService Log => SystemLog;


    /// <summary>
    /// The Mail service, used to send mails
    /// </summary>
    public IMailService Mail => _mail.Get(GetKitService<IMailService>);
    private readonly GetOnce<IMailService> _mail = new();


    /// <summary>
    /// The Page service, used to set headers, activate features etc.
    /// </summary>
    public IPageService Page => _page.Get(GetKitService<IPageService>);
    private readonly GetOnce<IPageService> _page = new();


    /// <summary>
    /// The Render service, used to render one or more dynamic content within other content
    /// </summary>
    public IRenderService Render => _render.Get(GetKitService<IRenderService>);
    private readonly GetOnce<IRenderService> _render = new();

    /// <summary>
    /// The Secure Data service - mainly for reading / decrypting secrets. 
    /// </summary>
    public ISecureDataService SecureData => _secureData.Get(GetKitService<ISecureDataService>);
    private readonly GetOnce<ISecureDataService> _secureData = new();

    /// <summary>
    /// The Razor-Blade Scrub service, used to clean up HTML.
    /// See [](xref:ToSic.Razor.Blade.IScrub)
    /// </summary>
    public IScrub Scrub => _scrub.Get(GetKitService<IScrub>);
    private readonly GetOnce<IScrub> _scrub = new();


    /// <summary>
    /// The toolbar service, used to generate advanced toolbars
    /// </summary>
    public IToolbarService Toolbar => _toolbar.Get(GetKitService<IToolbarService>);
    private readonly GetOnce<IToolbarService> _toolbar = new();

    [PrivateApi("Experimental in v15.03")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public IUserService User => _users.Get(GetKitService<IUserService>);
    private readonly GetOnce<IUserService> _users = new();
}