using ToSic.Eav;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Sxc.Plumbing;
using ToSic.Sxc.Web;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public abstract class ResponsiveBase: HybridHtmlString
    {
        protected ResponsiveBase(
            ImageService imgService, 
            string url, 
            object settings, 
            // ReSharper disable once UnusedParameter.Local
            string noParamOrder = Parameters.Protector, 
            object factor = null, 
            string srcSet = null,
            string imgAlt = null,
            string imgClass = null,
            string logName = Constants.SxcLogName + ".IPSBas"
            )
        {
            ImgService = imgService;
            FactorParam = factor;
            SrcSetParam = srcSet;
            ImgAlt = imgAlt;
            ImgClass = imgClass;
            ImgLinker = imgService.ImgLinker;
            UrlOriginal = url;
            Settings = PrepareResizeSettings(settings, factor, srcSet);

        }
        protected readonly ImgResizeLinker ImgLinker;
        protected readonly ImageService ImgService;
        protected readonly object FactorParam;
        protected readonly string SrcSetParam;
        protected readonly string ImgAlt;
        protected readonly string ImgClass;
        protected readonly string UrlOriginal;

        public string Url => _url ?? (_url = ImgLinker.ImageOnly(UrlOriginal, new ResizeSettings(Settings, false)));
        private string _url;
        internal IResizeSettings Settings { get; }


        protected IResizeSettings PrepareResizeSettings(object settings, object factor, string srcset)
        {
            // 1. Prepare Settings
            if (settings is IResizeSettings resizeSettings)
                ((ResizeSettings)resizeSettings).Factor = ParseObject.DoubleOrNullWithCalculation(factor) ?? resizeSettings.Factor;
            else
                resizeSettings = ImgLinker.ResizeParamMerger.BuildResizeSettings(settings, factor: factor, srcset: true);

            if (srcset != null) ((ResizeSettings)resizeSettings).SrcSet = srcset;

            return resizeSettings;
        }

        /// <summary>
        /// ToString must be specified by each implementation
        /// </summary>
        /// <returns></returns>
        public abstract override string ToString();

        public virtual Img Img
        {
            get
            {
                if (_imgTag != null) return _imgTag;

                _imgTag = Tag.Img().Src(Url);

                // Only add these if they were really specified
                if (ImgAlt != null) 
                    _imgTag.Alt(ImgAlt);
                if (ImgClass != null) _imgTag.Class(ImgClass);
                return _imgTag;
            }
        }

        private Img _imgTag;

    }
}
