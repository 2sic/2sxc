using Connect.Koi;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Default ServiceKit for 2sxc v14.
    /// </summary>
    /// <remarks>
    /// * History: Added v14.04
    /// </remarks>
    [PublicApi]
    public class ServiceKit14: ServiceKit
    {
        /// <summary>
        /// The ADAM Service, used to retrieve files and maybe more. 
        /// </summary>
        public IAdamService Adam => _adam.Get(GetService<IAdamService>);
        private readonly GetOnce<IAdamService> _adam = new GetOnce<IAdamService>();

        /// <summary>
        /// The Convert Service, used to convert any kind of data type to another data type
        /// </summary>
        public IConvertService Convert => _convert.Get(GetService<IConvertService>);
        private readonly GetOnce<IConvertService> _convert = new GetOnce<IConvertService>();

        /// <summary>
        /// The Koi CSS Service, used to detect the current CSS framework and other features.
        /// See [ICss](xref:Connect.Koi.ICss)
        /// </summary>
        public ICss Css => _css.Get(GetService<ICss>);
        private readonly GetOnce<ICss> _css = new GetOnce<ICss>();


        // Wait till we have a signature without the IEntity, but more an IHasEntity or something
        ///// <summary>
        ///// The TODO Service, used to detect the current CSS framework and other features.
        ///// See [ICss](xref:Connect.Koi.ICss)
        ///// </summary>
        //public IConvertToEavLight Todo => _convEavLight.Get(GetService<IConvertToEavLight>);
        //private readonly ValueGetOnce<IConvertToEavLight> _convEavLight = new ValueGetOnce<IConvertToEavLight>();

        /// <summary>
        /// The Edit service, same as the main Edit service
        /// </summary>
        public IEditService Edit => _edit.Get(GetService<IEditService>);
        private readonly GetOnce<IEditService> _edit = new GetOnce<IEditService>();


        /// <summary>
        /// The Features services, used to check if features are enabled
        /// </summary>
        public IFeaturesService Feature => _features.Get(GetService<IFeaturesService>);
        private readonly GetOnce<IFeaturesService> _features = new GetOnce<IFeaturesService>();


        /// <summary>
        /// The Images service, used to create `img` and `picture` tags
        /// </summary>
        public IImageService Image => _image.Get(GetService<IImageService>);
        private readonly GetOnce<IImageService> _image = new GetOnce<IImageService>();


        /// <summary>
        /// The JSON service, used to convert data to-and-from JSON
        /// </summary>
        public IJsonService Json => _json.Get(GetService<IJsonService>);
        private readonly GetOnce<IJsonService> _json = new GetOnce<IJsonService>();


        /// <summary>
        /// The JSON service, used to convert data to-and-from JSON
        /// </summary>
        public ILinkService Link => _link.Get(GetService<ILinkService>);
        private readonly GetOnce<ILinkService> _link = new GetOnce<ILinkService>();

        /// <summary>
        /// The System Log service, used to add log messages to the system (Dnn/Oqtane)
        /// </summary>
        public ILogService Log => _sysLog.Get(GetService<ILogService>);
        private readonly GetOnce<ILogService> _sysLog = new GetOnce<ILogService>();


        /// <summary>
        /// The Mail service, used to send mails
        /// </summary>
        public IMailService Mail => _mail.Get(GetService<IMailService>);
        private readonly GetOnce<IMailService> _mail = new GetOnce<IMailService>();


        /// <summary>
        /// The Page service, used to set headers, activate features etc.
        /// </summary>
        public IPageService Page => _page.Get(GetService<IPageService>);
        private readonly GetOnce<IPageService> _page = new GetOnce<IPageService>();


        /// <summary>
        /// The Render service, used to render one or more dynamic content within other content
        /// </summary>
        public IRenderService Render => _render.Get(GetService<IRenderService>);
        private readonly GetOnce<IRenderService> _render = new GetOnce<IRenderService>();

        /// <summary>
        /// The Secure Data Service - mainly for reading / decrypting secrets. 
        /// </summary>
        public ISecureDataService SecureData => _secureData.Get(GetService<ISecureDataService>);
        private readonly GetOnce<ISecureDataService> _secureData = new GetOnce<ISecureDataService>();

        /// <summary>
        /// The Razor-Blade Scrub service, used to clean up HTML.
        /// See [IScrub](xref:ToSic.Razor.Blade.IScrub)
        /// </summary>
        public IScrub Scrub => _scrub.Get(GetService<IScrub>);
        private readonly GetOnce<IScrub> _scrub = new GetOnce<IScrub>();


        /// <summary>
        /// The toolbar service, used to generate advanced toolbars
        /// </summary>
        public IToolbarService Toolbar => _toolbar.Get(GetService<IToolbarService>);
        private readonly GetOnce<IToolbarService> _toolbar = new GetOnce<IToolbarService>();
    }

}
