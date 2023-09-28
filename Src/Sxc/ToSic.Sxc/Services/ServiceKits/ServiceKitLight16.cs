using System;
using ToSic.Eav.Apps;
using ToSic.Eav.LookUp;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Services.DataServices;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Lightweight ServiceKit for 2sxc v15.
    /// It's primarily used in dynamic code which runs standalone, without a module context.
    ///
    /// Example: Custom DataSources can run anywhere without actually being inside a module or content-block.
    /// In such scenarios, certain services like the <see cref="IPageService"/> would not be able to perform any real work.
    /// </summary>
    /// <remarks>
    /// * History: Added v15.06 - still WIP
    /// </remarks>
    [PrivateApi]
    public class ServiceKitLight16 : ServiceBase
    {

        [PrivateApi("Public constructor for DI")]
        public ServiceKitLight16(IServiceProvider serviceProvider) : base("Sxc.Kit15")
        {
            _serviceProvider = serviceProvider;
        }
        private readonly IServiceProvider _serviceProvider;
        [PrivateApi]
        private TService GetService<TService>() => _serviceProvider.Build<TService>(Log);

        [PrivateApi]
        internal ServiceKitLight16 Setup(IAppIdentity appIdentity, Func<ILookUpEngine> getLookup)
        {
            _appIdentity = appIdentity;
            _getLookup = getLookup;
            return this;
        }

        private IAppIdentity _appIdentity;
        private Func<ILookUpEngine> _getLookup;

        ///// <summary>
        ///// The ADAM Service, used to retrieve files and maybe more. 
        ///// </summary>
        //public IAdamService Adam => _adam.Get(GetService<IAdamService>);
        //private readonly GetOnce<IAdamService> _adam = new GetOnce<IAdamService>();

        ///// <summary>
        ///// The CMS Service - WIP
        ///// </summary>
        //[PrivateApi("Still WIP v15")]
        //public ICmsService Cms => _cms.Get(GetService<ICmsService>);
        //private readonly GetOnce<ICmsService> _cms = new GetOnce<ICmsService>();

        /// <summary>
        /// The Convert Service, used to convert any kind of data type to another data type
        /// </summary>
        public IConvertService Convert => _convert.Get(GetService<IConvertService>);
        private readonly GetOnce<IConvertService> _convert = new GetOnce<IConvertService>();

        ///// <summary>
        ///// The Koi CSS Service, used to detect the current CSS framework and other features.
        ///// See [ICss](xref:Connect.Koi.ICss)
        ///// </summary>
        //public ICss Css => _css.Get(GetService<ICss>);
        //private readonly GetOnce<ICss> _css = new GetOnce<ICss>();


        /// <summary>
        /// The Edit service, same as the main Edit service
        /// </summary>
        public IDataService Data => _data.Get(() =>
        {
            var dss = GetService<IDataService>();
            (dss as DataService)?.Setup(_appIdentity, _getLookup);
            return dss;
        });
        private readonly GetOnce<IDataService> _data = new GetOnce<IDataService>();

        ///// <summary>
        ///// The Edit service, same as the main Edit service
        ///// </summary>
        //public IEditService Edit => _edit.Get(GetService<IEditService>);
        //private readonly GetOnce<IEditService> _edit = new GetOnce<IEditService>();


        /// <summary>
        /// The Features services, used to check if features are enabled
        /// </summary>
        public IFeaturesService Feature => _features.Get(GetService<IFeaturesService>);
        private readonly GetOnce<IFeaturesService> _features = new GetOnce<IFeaturesService>();

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
        public IHtmlTagsService HtmlTags => _ht.Get(GetService<IHtmlTagsService>);
        private readonly GetOnce<IHtmlTagsService> _ht = new GetOnce<IHtmlTagsService>();

        ///// <summary>
        ///// The Images service, used to create `img` and `picture` tags
        ///// </summary>
        //public IImageService Image => _image.Get(GetService<IImageService>);
        //private readonly GetOnce<IImageService> _image = new GetOnce<IImageService>();


        /// <summary>
        /// The JSON service, used to convert data to-and-from JSON
        /// </summary>
        public IJsonService Json => _json.Get(GetService<IJsonService>);
        private readonly GetOnce<IJsonService> _json = new GetOnce<IJsonService>();


        ///// <summary>
        ///// The JSON service, used to convert data to-and-from JSON
        ///// </summary>
        //public ILinkService Link => _link.Get(GetService<ILinkService>);
        //private readonly GetOnce<ILinkService> _link = new GetOnce<ILinkService>();

        /// <summary>
        /// The System Log service, used to add log messages to the system (Dnn/Oqtane)
        /// </summary>
        public ISystemLogService SystemLog => _sysLog.Get(GetService<ISystemLogService>);
        private readonly GetOnce<ISystemLogService> _sysLog = new GetOnce<ISystemLogService>();


        ///// <summary>
        ///// The Mail service, used to send mails
        ///// </summary>
        //public IMailService Mail => _mail.Get(GetService<IMailService>);
        //private readonly GetOnce<IMailService> _mail = new GetOnce<IMailService>();


        ///// <summary>
        ///// The Page service, used to set headers, activate features etc.
        ///// </summary>
        //public IPageService Page => _page.Get(GetService<IPageService>);
        //private readonly GetOnce<IPageService> _page = new GetOnce<IPageService>();


        ///// <summary>
        ///// The Render service, used to render one or more dynamic content within other content
        ///// </summary>
        //public IRenderService Render => _render.Get(GetService<IRenderService>);
        //private readonly GetOnce<IRenderService> _render = new GetOnce<IRenderService>();

        /// <summary>
        /// The Secure Data service - mainly for reading / decrypting secrets. 
        /// </summary>
        public ISecureDataService SecureData => _secureData.Get(GetService<ISecureDataService>);
        private readonly GetOnce<ISecureDataService> _secureData = new GetOnce<ISecureDataService>();

        /// <summary>
        /// The Razor-Blade Scrub service, used to clean up HTML.
        /// See [](xref:ToSic.Razor.Blade.IScrub)
        /// </summary>
        public IScrub Scrub => _scrub.Get(GetService<IScrub>);
        private readonly GetOnce<IScrub> _scrub = new GetOnce<IScrub>();


        ///// <summary>
        ///// The toolbar service, used to generate advanced toolbars
        ///// </summary>
        //public IToolbarService Toolbar => _toolbar.Get(GetService<IToolbarService>);
        //private readonly GetOnce<IToolbarService> _toolbar = new GetOnce<IToolbarService>();

        //[PrivateApi("Experimental in v15.03")]
        //public IUsersService Users => _users.Get(GetService<IUsersService>);
        //private readonly GetOnce<IUsersService> _users = new GetOnce<IUsersService>();
    }

}
