using ToSic.Eav;
using ToSic.Eav.Logging;
using ToSic.Sxc.Code;
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

        #endregion

        #region Settings Handling

        /// <summary>
        /// Use the given settings or try to use the default content-settings if available
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private object GetBestSettings(object settings)
        {
            return settings == null || settings is bool boolSettings && boolSettings
                ? _codeRootOrNull?.Settings?.Images?.Content
                : settings;
        }

        #endregion



        public IResponsivePicture Picture(
            string url, 
            string noParamOrder = Parameters.Protector,
            object settings = null,
            object factor = null, 
            string srcset = null, 
            string imgAlt = null, 
            string imgClass = null
        ) => new ResponsivePicture(this, _features, url, GetBestSettings(settings), factor: factor, srcSet: srcset);

        public IResponsiveImage Img(
            string url,
            string noParamOrder = Parameters.Protector,
            object settings = null,
            object factor = null,
            string srcset = null,
            string imgAlt = null,
            string imgClass = null
        ) => new ResponsiveImage(this, url, GetBestSettings(settings), factor: factor, srcSet: srcset, imgAlt: imgAlt, imgClass: imgClass);

        public IHybridHtmlString SrcSet(
            string url, 
            object settings = null,
            string noParamOrder = Parameters.Protector,
            object factor = null, 
            string srcSet = null
        ) => new HybridHtmlString(ImgLinker.Image(url, GetBestSettings(settings), factor: factor, srcset: (srcSet as object) ?? true));

    }
}
