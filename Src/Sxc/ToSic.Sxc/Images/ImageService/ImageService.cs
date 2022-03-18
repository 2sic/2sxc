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
            Features = features;
            ImgLinker = imgLinker.Init(Log);
        }

        internal ImgResizeLinker ImgLinker { get; }
        internal IFeaturesService Features { get; }

        public void ConnectToRoot(IDynamicCodeRoot codeRoot) => _codeRootOrNull = codeRoot;
        private IDynamicCodeRoot _codeRootOrNull;

        //public ImageServiceSettings Settings { get; } = new ImageServiceSettings();

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
        private RecipeSet ToMRS(object value) => RecipeSet.Parse(value)?.InitAfterLoad();

        #endregion



        public IResponsivePicture Picture(
            string url, 
            string noParamOrder = Parameters.Protector,
            object settings = default,
            object factor = default,
            string imgAlt = default,
            string imgClass = default,
            object recipe = default
        ) => new ResponsivePicture(this, url, GetBestSettings(settings), factor: factor, mrs: ToMRS(recipe), imgAlt: imgAlt, imgClass: imgClass);

        public IResponsiveImage Img(
            string url,
            string noParamOrder = Parameters.Protector,
            object settings = default,
            object factor = default,
            string imgAlt = default,
            string imgClass = default,
            object recipe = default
        ) => new ResponsiveImage(this, url, GetBestSettings(settings), factor: factor, mrs: ToMRS(recipe), imgAlt: imgAlt, imgClass: imgClass);

        public IHybridHtmlString SrcSet(string url,
            object settings = null,
            string noParamOrder = "Rule: All params must be named (https://r.2sxc.org/named-params)",
            object factor = null,
            object recipe = null
        ) => new HybridHtmlString(ImgLinker.SrcSet(url, MergeSettings(settings, factor: factor, recipe: recipe), SrcSetType.Img));

        private ResizeSettings MergeSettings(
            object settings = null,
            string noParamOrder = Parameters.Protector,
            object factor = null, 
            object recipe = null
        ) => ImgLinker.ResizeParamMerger.BuildResizeSettings(GetBestSettings(settings), factor: factor, advanced: ToMRS(recipe));
    }
}
