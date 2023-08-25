using Connect.Koi;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context.Keys;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Default ServiceKit for 2sxc v14.
    /// </summary>
    /// <remarks>
    /// * History: Added v14.04
    /// </remarks>
    [PublicApi]
    public class ServiceKit16: ServiceKit
    {
        [PrivateApi("Public constructor for DI")]
        public ServiceKit16() : base("Sxc.Kit16")
        {
        }

        /// <inheritdoc cref="ServiceKit14.Adam"/>
        public IAdamService Adam => _adam.Get(GetService<IAdamService>);
        private readonly GetOnce<IAdamService> _adam = new GetOnce<IAdamService>();

        /// <summary>
        /// The CMS Service - WIP
        /// </summary>
        [PrivateApi("Not yet for public use, as API is not yet public")]
        internal ICmsService Cms => _cms.Get(GetService<ICmsService>);
        private readonly GetOnce<ICmsService> _cms = new GetOnce<ICmsService>();

        /// <summary>
        /// The Convert Service, used to convert any kind of data type to another data type
        /// </summary>
        public IConvertService16 Convert => _convert.Get(GetService<IConvertService16>);
        private readonly GetOnce<IConvertService16> _convert = new GetOnce<IConvertService16>();

        /// <inheritdoc cref="ServiceKit14.Css"/>
        public ICss Css => _css.Get(GetService<ICss>);
        private readonly GetOnce<ICss> _css = new GetOnce<ICss>();


        /// <summary>
        /// The Data service to get DataSources and similar.
        /// </summary>
        /// <remarks>
        /// * added in v15.06
        /// </remarks>
        [PrivateApi("WIP not yet public for v15 - added v15.06")]
        public IDataService Data => _data.Get(GetService<IDataService>);
        private readonly GetOnce<IDataService> _data = new GetOnce<IDataService>();

        /// <inheritdoc cref="ServiceKit14.Edit"/>
        public IEditService Edit => _edit.Get(GetService<IEditService>);
        private readonly GetOnce<IEditService> _edit = new GetOnce<IEditService>();


        /// <inheritdoc cref="ServiceKit14.Feature"/>
        public IFeaturesService Feature => _features.Get(GetService<IFeaturesService>);
        private readonly GetOnce<IFeaturesService> _features = new GetOnce<IFeaturesService>();

        /// <inheritdoc cref="ServiceKit14.HtmlTags"/>
        public IHtmlTagsService HtmlTags => _ht.Get(GetService<IHtmlTagsService>);
        private readonly GetOnce<IHtmlTagsService> _ht = new GetOnce<IHtmlTagsService>();

        /// <inheritdoc cref="ServiceKit14.Image"/>
        public IImageService Image => _image.Get(GetService<IImageService>);
        private readonly GetOnce<IImageService> _image = new GetOnce<IImageService>();


        /// <inheritdoc cref="ServiceKit14.Json"/>
        public IJsonService Json => _json.Get(GetService<IJsonService>);
        private readonly GetOnce<IJsonService> _json = new GetOnce<IJsonService>();


        /// <inheritdoc cref="IDynamicCode.Link" />
        public ILinkService Link => _link.Get(GetService<ILinkService>);
        private readonly GetOnce<ILinkService> _link = new GetOnce<ILinkService>();

        /// <inheritdoc cref="ServiceKit14.SystemLog"/>
        public ISystemLogService SystemLog => _sysLog.Get(GetService<ISystemLogService>);
        private readonly GetOnce<ISystemLogService> _sysLog = new GetOnce<ISystemLogService>();

        // Removed for v16
        ///// <summary>
        ///// Note that this was used in Mobius / Events in a few releases, so we can't just change it.
        ///// If we create a Kit15, this should be removed
        ///// </summary>
        //[PrivateApi("was the official name before v15.06, probably never used publicly, but should stay in for a while")]
        //public new ISystemLogService Log => SystemLog;


        /// <inheritdoc cref="ServiceKit14.Mail"/>
        public IMailService Mail => _mail.Get(GetService<IMailService>);
        private readonly GetOnce<IMailService> _mail = new GetOnce<IMailService>();


        /// <inheritdoc cref="ServiceKit14.Page"/>
        public IPageService Page => _page.Get(GetService<IPageService>);
        private readonly GetOnce<IPageService> _page = new GetOnce<IPageService>();


        /// <inheritdoc cref="ServiceKit14.Render"/>
        public IRenderService Render => _render.Get(GetService<IRenderService>);
        private readonly GetOnce<IRenderService> _render = new GetOnce<IRenderService>();

        /// <inheritdoc cref="ServiceKit14.SecureData"/>
        public ISecureDataService SecureData => _secureData.Get(GetService<ISecureDataService>);
        private readonly GetOnce<ISecureDataService> _secureData = new GetOnce<ISecureDataService>();

        /// <inheritdoc cref="ServiceKit14.Scrub"/>
        public IScrub Scrub => _scrub.Get(GetService<IScrub>);
        private readonly GetOnce<IScrub> _scrub = new GetOnce<IScrub>();


        /// <inheritdoc cref="ServiceKit14.Toolbar"/>
        public IToolbarService Toolbar => _toolbar.Get(GetService<IToolbarService>);
        private readonly GetOnce<IToolbarService> _toolbar = new GetOnce<IToolbarService>();

        /// <inheritdoc cref="ServiceKit14.User"/>
        [PrivateApi("Experimental in v15.03")]
        public IUserService User => _users.Get(GetService<IUserService>);
        private readonly GetOnce<IUserService> _users = new GetOnce<IUserService>();

        // v16 new Keys
        [InternalApi_DoNotUse_MayChangeWithoutNotice("WIP v16.04")]
        public IKeyService Key => _keys ?? (_keys = new KeyService());
        private IKeyService _keys;
    }

}
