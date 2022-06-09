using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Services
{
    public class Kit14: KitBase
    {
        /// <summary>
        /// The Convert Service, used to convert any kind of data type to another data type
        /// </summary>
        public IConvertService Convert => _convert.Get(GetService<IConvertService>);
        private readonly ValueGetOnce<IConvertService> _convert = new ValueGetOnce<IConvertService>();

        /// <summary>
        /// The Edit service, same as the main Edit service
        /// </summary>
        public IEditService Edit => _edit.Get(GetService<IEditService>);
        private readonly ValueGetOnce<IEditService> _edit = new ValueGetOnce<IEditService>();


        /// <summary>
        /// The Features services, used to check if features are enabled
        /// </summary>
        public IFeaturesService Features => _features.Get(GetService<IFeaturesService>);
        private readonly ValueGetOnce<IFeaturesService> _features = new ValueGetOnce<IFeaturesService>();

        /// <summary>
        /// The Images service, used to create `img` and `picture` tags
        /// </summary>
        public IImageService Images => _image.Get(GetService<IImageService>);
        private readonly ValueGetOnce<IImageService> _image = new ValueGetOnce<IImageService>();

        /// <summary>
        /// The JSON service, used to convert data to-and-from JSON
        /// </summary>
        public IJsonService Json => _json.Get(GetService<IJsonService>);
        private readonly ValueGetOnce<IJsonService> _json = new ValueGetOnce<IJsonService>();

        /// <summary>
        /// The JSON service, used to convert data to-and-from JSON
        /// </summary>
        public ILinkService Link => _link.Get(GetService<ILinkService>);
        private readonly ValueGetOnce<ILinkService> _link = new ValueGetOnce<ILinkService>();

        /// <summary>
        /// The Mail service, used to send mails
        /// </summary>
        public IMailService Mail => _mail.Get(GetService<IMailService>);
        private readonly ValueGetOnce<IMailService> _mail = new ValueGetOnce<IMailService>();

        /// <summary>
        /// The Render service, used to render one or more dynamic content within other content
        /// </summary>
        public IRenderService Render => _render.Get(GetService<IRenderService>);
        private readonly ValueGetOnce<IRenderService> _render = new ValueGetOnce<IRenderService>();

        /// <summary>
        /// The Page service, used to set headers, activate features etc.
        /// </summary>
        public IPageService Page => _page.Get(GetService<IPageService>);
        private readonly ValueGetOnce<IPageService> _page = new ValueGetOnce<IPageService>();

        /// <summary>
        /// The toolbar service, used to generate advanced toolbars
        /// </summary>
        public IToolbarService Toolbar => _toolbar.Get(GetService<IToolbarService>);
        private readonly ValueGetOnce<IToolbarService> _toolbar = new ValueGetOnce<IToolbarService>();
    }

}
