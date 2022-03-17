using ToSic.Eav;
using ToSic.Eav.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Images
{
    public partial class ImageService: HasLog<ImageService>, IImageService, INeedsDynamicCodeRoot
    {
        #region Constructor and Inits

        public ImageService(ImgResizeLinker imgLinker, IFeaturesService features) : base(Constants.SxcLogName + ".ImgSvc")
        {
            _features = features;
            ImgLinker = imgLinker.Init(Log);
        }

        internal ImgResizeLinker ImgLinker { get; }
        private readonly IFeaturesService _features;

        public void ConnectToRoot(IDynamicCodeRoot codeRoot) => _codeRootOrNull = codeRoot;
        private IDynamicCodeRoot _codeRootOrNull;

        public ImageServiceSettings Settings { get; } = new ImageServiceSettings();

        #endregion

        #region Settings Handling

        /// <summary>
        /// Use the given settings or try to use the default content-settings if available
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private object GetBestSettings(object settings)
        {
            if (settings == null || settings is bool boolSettings && boolSettings)
                return _codeRootOrNull?.Settings?.Images?.Content;

            if (settings is string strName && !string.IsNullOrWhiteSpace(strName))
                return (_codeRootOrNull?.Settings?.Images as ICanGetByName)?.Get(strName);

            return settings;
        }

        /// <summary>
        /// Convert to Multi-Resize Settings
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private MultiResizeSettings ToMRS(object value) => MultiResizeSettings.Parse(value)?.InitAfterLoad();

        #endregion



        public IResponsivePicture Picture(
            string url, 
            string noParamOrder = Parameters.Protector,
            object settings = default,
            object factor = default,
            string imgAlt = default,
            string imgClass = default,
            object rules = default
        ) => new ResponsivePicture(this, _features, url, GetBestSettings(settings), factor: factor, mrs: ToMRS(rules), imgAlt: imgAlt, imgClass: imgClass);

        public IResponsiveImage Img(
            string url,
            string noParamOrder = Parameters.Protector,
            object settings = default,
            object factor = default,
            string imgAlt = default,
            string imgClass = default,
            object rules = default
        ) => new ResponsiveImage(this, url, GetBestSettings(settings), factor: factor, mrs: ToMRS(rules), imgAlt: imgAlt, imgClass: imgClass);

        public IHybridHtmlString SrcSet(
            string url, 
            object settings = null,
            string noParamOrder = Parameters.Protector,
            object factor = null, 
            string srcset = null
            // TODO: RULES
        ) => new HybridHtmlString(ImgLinker.SrcSet(url, MergeSettings(settings, factor: factor, srcset: srcset), SrcSetType.Img));

        private ResizeSettings MergeSettings(
            object settings = null,
            string noParamOrder = Parameters.Protector,
            object factor = null, 
            string srcset = null
            // TODO: RULES
        ) => ImgLinker.ResizeParamMerger.BuildResizeSettings((GetBestSettings(settings), factor: factor, srcset: srcset as object ?? true));
    }
}
