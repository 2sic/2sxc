using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Images
{
    public partial class ImageService: HasLog, IImageService, INeedsDynamicCodeRoot
    {
        #region Constructor and Inits

        public ImageService(ImgResizeLinker imgLinker, IFeaturesService features) : base(Constants.SxcLogName + ".ImgSvc")
        {
            Features = features.Init(Log);
            ImgLinker = imgLinker.Init(Log);
        }
        internal ImgResizeLinker ImgLinker { get; }
        internal IFeaturesService Features { get; }

        public void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            _codeRootOrNull = codeRoot;
            Log.LinkTo(_codeRootOrNull.Log);
        }
        private IDynamicCodeRoot _codeRootOrNull;
        internal IEditService EditOrNull => _codeRootOrNull?.Edit;

        internal IToolbarService ToolbarOrNull => _toolbarSvc.Get(() => _codeRootOrNull?.GetService<IToolbarService>());
        private readonly GetOnce<IToolbarService> _toolbarSvc = new GetOnce<IToolbarService>();

        #endregion

        #region Settings Handling

        /// <summary>
        /// Use the given settings or try to use the default content-settings if available
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private object GetBestSettings(object settings)
        {
            var wrapLog = Log.Fn<object>(Debug);
            if (settings == null || settings is bool boolSettings && boolSettings)
                return wrapLog.Return(GetCodeRootSettingsByName("Content"), "null/default");

            if (settings is string strName && !string.IsNullOrWhiteSpace(strName))
                return wrapLog.Return(GetCodeRootSettingsByName(strName), $"name: {strName}");

            return wrapLog.Return(settings, "unchanged");
        }

        private dynamic GetCodeRootSettingsByName(string strName)
        {
            var wrapLog = Log.Fn<object>(Debug, strName, message: $"code root: {_codeRootOrNull != null}");
            var result = (_codeRootOrNull?.Settings?.Images as ICanGetByName)?.Get(strName);
            return wrapLog.Return(result, $"found: {result != null}");
        }

        /// <summary>
        /// Convert to Multi-Resize Settings
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private AdvancedSettings ToAdv(object value) => AdvancedSettings.Parse(value);

        #endregion

        /// <inheritdoc />
        public IResponsiveImage Img(
            object link = null,
            object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null,
            object width = default,
            string imgAlt = null,
            string imgClass = null,
            object recipe = null)
            => new ResponsiveImage(this,
                new ResponsiveParams(nameof(Img), link, noParamOrder, Settings(settings, factor: factor, width: width, recipe: recipe), imgAlt, imgClass))
                .Init(Log);


        /// <inheritdoc />
        public IResponsiveImage ImgOrPic(
            object link = null,
            object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null,
            object width = default,
            string imgAlt = null,
            string imgClass = null,
            object recipe = null)
        {
            var respParams = new ResponsiveParams(nameof(ImgOrPic), link, noParamOrder, Settings(settings, factor: factor, width: width, recipe: recipe), imgAlt, imgClass);
            var path = respParams.Link.Url;
            var format = GetFormat(path);
            return format.ResizeFormats.Any()
                ? (IResponsiveImage)new ResponsivePicture(this, respParams).Init(Log)
                : new ResponsiveImage(this, respParams).Init(Log);
        }


        /// <inheritdoc />
        public IResponsivePicture Picture(
            object link = null,
            object settings = null,
            string noParamOrder = Eav.Parameters.Protector, 
            object factor = null,
            object width = default,
            string imgAlt = null,
            string imgClass = null,
            object recipe = null)
            => new ResponsivePicture(this,
                new ResponsiveParams(nameof(Picture), link, noParamOrder, Settings(settings, factor: factor, width: width, recipe: recipe), imgAlt, imgClass))
                .Init(Log);

        /// <inheritdoc />
        public bool Debug
        {
            get => _debug;
            set
            {
                _debug = value;
                ImgLinker.Debug = value;
                Features.Debug = value;
            }
        }
        private bool _debug;
    }
}
