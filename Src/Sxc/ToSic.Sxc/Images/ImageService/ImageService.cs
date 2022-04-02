using System.Linq;
using ToSic.Eav.Logging;
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

        #endregion

        #region Settings Handling

        /// <summary>
        /// Use the given settings or try to use the default content-settings if available
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private object GetBestSettings(object settings)
        {
            var wrapLog = Log.SafeCall<object>(Debug);
            if (settings == null || settings is bool boolSettings && boolSettings)
                return wrapLog("null/default", GetCodeRootSettingsByName("Content"));

            if (settings is string strName && !string.IsNullOrWhiteSpace(strName))
                return wrapLog($"name: {strName}", GetCodeRootSettingsByName(strName));

            return wrapLog("unchanged", settings);
        }

        private dynamic GetCodeRootSettingsByName(string strName)
        {
            var wrapLog = Log.SafeCall<object>(Debug, strName, message: $"code root: {_codeRootOrNull != null}");
            var result = (_codeRootOrNull?.Settings?.Images as ICanGetByName)?.Get(strName);
            return wrapLog($"found: {result != null}", result);
        }

        /// <summary>
        /// Convert to Multi-Resize Settings
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private AdvancedSettings ToAdv(object value) => AdvancedSettings.Parse(value);

        #endregion

        /// <inheritdoc />
        public IResponsiveImage Img(object link = null,
            string noParamOrder = Eav.Parameters.Protector,
            object settings = null,
            object factor = null,
            string imgAlt = null,
            string imgClass = null,
            object recipe = null)
            => new ResponsiveImage(this,
                new ResponsiveParams(nameof(Img), link, noParamOrder, EnsureSettings(GetBestSettings(settings), factor, recipe), imgAlt, imgClass))
                .Init(Log);


        /// <inheritdoc />
        public IResponsiveImage ImgOrPic(object link = null,
            string noParamOrder = Eav.Parameters.Protector,
            object settings = null,
            object factor = null,
            string imgAlt = null,
            string imgClass = null,
            object recipe = null)
        {
            var respParams = new ResponsiveParams(nameof(ImgOrPic), link, noParamOrder, EnsureSettings(GetBestSettings(settings), factor, recipe), imgAlt, imgClass);
            var path = respParams.Link.Url;
            var format = GetFormat(path);
            return format.ResizeFormats.Any()
                ? (IResponsiveImage)new ResponsivePicture(this, respParams).Init(Log)
                : new ResponsiveImage(this, respParams).Init(Log);
        }


        /// <inheritdoc />
        public IResponsivePicture Picture(object link = null,
            string noParamOrder = Eav.Parameters.Protector, 
            object settings = null,
            object factor = null,
            string imgAlt = null,
            string imgClass = null,
            object recipe = null)
            => new ResponsivePicture(this,
                new ResponsiveParams(nameof(Picture), link, noParamOrder, EnsureSettings(GetBestSettings(settings), factor, recipe), imgAlt, imgClass))
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
