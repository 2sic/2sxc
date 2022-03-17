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
            //string srcSet = null,
            object rules = null,
            string imgAlt = null,
            string imgClass = null,
            string logName = Constants.SxcLogName + ".IPSBas"
            )
        {
            ImgService = imgService;
            FactorParam = factor;
            //SrcSetParam = srcSet;
            ImgAlt = imgAlt;
            ImgClass = imgClass;
            ImgLinker = imgService.ImgLinker;
            UrlOriginal = url;
            Settings = PrepareResizeSettings(settings, factor, /*srcSet,*/ rules);

        }
        protected readonly ImgResizeLinker ImgLinker;
        protected readonly ImageService ImgService;
        protected readonly object FactorParam;
        //protected readonly string SrcSetParam;
        protected readonly string ImgAlt;
        protected readonly string ImgClass;
        protected readonly string UrlOriginal;

        public string Url => ThisResize.Url;

        protected OneResize ThisResize => _thisResize ?? (_thisResize = ImgLinker.ImageOnly(UrlOriginal, Settings, SrcSetType.ImgSrc));
        private OneResize _thisResize;

        internal ResizeSettings Settings { get; }


        protected ResizeSettings PrepareResizeSettings(object settings, object factor, object rules)
        {
            // 0. Pre-check multi-rules
            var probablyMultiRules = MultiResizeSettings.Parse(rules)?.InitAfterLoad();

            // 1. Prepare Settings
            if (settings is ResizeSettings resSettings)
            {
                // If we have a modified factor, make sure we have that (this will copy the settings)
                var newFactor = ParseObject.DoubleOrNullWithCalculation(factor);
                if (newFactor != null) resSettings = new ResizeSettings(resSettings, factor: newFactor.Value);
            }
            else
                resSettings = ImgLinker.ResizeParamMerger.BuildResizeSettings(settings, factor: factor, /*srcset: true,*/ allowMulti: true, advanced: probablyMultiRules);

            //must make a copy if we change a property, to not have side-effects
            if (probablyMultiRules != null)
            {
                resSettings = new ResizeSettings(resSettings, probablyMultiRules);
            }

            return resSettings;
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
                if (ImgAlt != null) _imgTag.Alt(ImgAlt);
                var settings = ImgService.Settings;
                var classToAdd = $"{ImgClass} {settings.ImageClass}";
                if (!string.IsNullOrWhiteSpace(classToAdd)) _imgTag.Class(classToAdd.Trim());

                if (settings.ImageSetWidth && ThisResize.Width != 0) _imgTag.Width(ThisResize.Width);
                if (settings.ImageSetHeight && ThisResize.Height != 0) _imgTag.Height(ThisResize.Height);

                var imgSizes = ThisResize.TagEnhancements?.Sizes;
                if (!string.IsNullOrWhiteSpace(imgSizes)) _imgTag.Sizes(imgSizes);

                return _imgTag;
            }
        }

        private Img _imgTag;

    }
}
