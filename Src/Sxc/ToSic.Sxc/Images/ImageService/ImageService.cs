using ToSic.Eav;
using ToSic.Eav.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

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
            var wrapLog = Log.Call<object>();
            if (settings == null || settings is bool boolSettings && boolSettings)
                return wrapLog("null/default", GetCodeRootSettingsByName("Content"));

            if (settings is string strName && !string.IsNullOrWhiteSpace(strName))
                return wrapLog($"name: {strName}", GetCodeRootSettingsByName(strName));

            return wrapLog("unchanged", settings);
        }

        private dynamic GetCodeRootSettingsByName(string strName)
        {
            var wrapLog = Log.Call<object>(strName, message: $"code root: {_codeRootOrNull != null}");
            var result = (_codeRootOrNull?.Settings?.Images as ICanGetByName)?.Get(strName);
            return wrapLog($"found: {result != null}", result);
        }

        /// <summary>
        /// Convert to Multi-Resize Settings
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private AdvancedSettings ToMRS(object value) => AdvancedSettings.Parse(value);

        #endregion



        public IResponsivePicture Picture(object link = null,
            string noParamOrder = "Rule: All params must be named (https://r.2sxc.org/named-params)",
            object settings = null,
            object factor = null,
            string imgAlt = null,
            string imgClass = null,
            object recipe = null)
            => new ResponsivePicture(this,
                new ResponsiveParams(nameof(Picture), link, noParamOrder, null, null, GetBestSettings(settings), factor, imgAlt, imgClass,
                    ToMRS(recipe)));

        public IResponsiveImage Img(object link = null,
            string noParamOrder = "Rule: All params must be named (https://r.2sxc.org/named-params)",
            object settings = null,
            object factor = null,
            string imgAlt = null,
            string imgClass = null,
            object recipe = null)
            => new ResponsiveImage(this,
                new ResponsiveParams(nameof(Img), link, noParamOrder, null, null, GetBestSettings(settings), factor, imgAlt, imgClass,
                    ToMRS(recipe)));


        // 2022-03-19 2dm - not ready yet
        //public IHybridHtmlString SrcSet(string url,
        //    object settings = null,
        //    string noParamOrder = "Rule: All params must be named (https://r.2sxc.org/named-params)",
        //    object factor = null,
        //    object recipe = null
        //) => new HybridHtmlString(ImgLinker.SrcSet(url, MergeSettings(settings, factor: factor, recipe: recipe), SrcSetType.Img));

        private ResizeSettings MergeSettings(
            object settings = null,
            string noParamOrder = Parameters.Protector,
            object factor = null, 
            object recipe = null
        ) => ImgLinker.ResizeParamMerger.BuildResizeSettings(GetBestSettings(settings), factor: factor, advanced: ToMRS(recipe));
    }
}
