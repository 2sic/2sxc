using System.Linq;
using ToSic.Eav.Configuration;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Sxc.Plumbing;
using ToSic.Sxc.Web;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public abstract class ResponsiveBase: HybridHtmlStringLog, IResponsiveImage
    {

        protected ResponsiveBase(ImageService imgService, ResponsiveParams responsiveParams, string logName)
            : base($"Img.{logName}")
        {
            Call = responsiveParams;
            ImgService = imgService;
            ImgLinker = imgService.ImgLinker;
            Settings = PrepareResizeSettings(Call.Settings, Call.Factor, Call.Advanced);
        }
        protected ResponsiveParams Call { get; }
        protected readonly ImgResizeLinker ImgLinker;
        protected readonly ImageService ImgService;

        protected OneResize ThisResize => _thisResize.Get(() => { 
            var t = ImgLinker.ImageOnly(Call.Link.Url, Settings, Call.Field);
            Log.SafeAdd(ImgService.Debug, $"{nameof(ThisResize)}: " + t?.Dump());
            return t;
        });
        private readonly PropertyToRetrieveOnce<OneResize> _thisResize = new PropertyToRetrieveOnce<OneResize>();


        internal ResizeSettings Settings { get; }


        protected ResizeSettings PrepareResizeSettings(object settings, object factor, AdvancedSettings advanced)
        {
            var wrapLog = Log.SafeCall<ResizeSettings>(ImgService.Debug);
            // 1. Prepare Settings
            if (settings is ResizeSettings resSettings)
            {
                // If we have a modified factor, make sure we have that (this will copy the settings)
                var newFactor = ParseObject.DoubleOrNullWithCalculation(factor);
                Log.SafeAdd(ImgService.Debug, $"Is {nameof(ResizeSettings)}, now with new factor: {newFactor}, will clone/init");
                resSettings = new ResizeSettings(resSettings, factor: newFactor ?? resSettings.Factor, advanced);
            }
            else
            {
                Log.SafeAdd(ImgService.Debug, $"Not {nameof(ResizeSettings)}, will create");
                resSettings = ImgLinker.ResizeParamMerger.BuildResizeSettings(settings, factor: factor, advanced: advanced);
            }

            return wrapLog("ok", resSettings);
        }

        /// <summary>
        /// ToString must be specified by each implementation
        /// </summary>
        /// <returns></returns>
        public abstract override string ToString();

        /// <inheritdoc />
        public virtual Img Img
        {
            get
            {
                if (_imgTag != null) return _imgTag;

                var wrapLog = Log.SafeCall<Img>(ImgService.Debug);
                _imgTag = Tag.Img().Src(Url);

                // Add all kind of attributes if specified
                var tag = ThisResize.Recipe;
                var dic = tag?.Attributes?
                    .Where(pair => !Recipe.SpecialProperties.Contains(pair.Key))
                    .ToDictionary(p => p.Key, p => p.Value); ;
                if (dic != null)
                {
                    Log.SafeAdd(ImgService.Debug, "will add properties from attributes");
                    foreach (var a in dic)
                        _imgTag.Attr(a.Key, a.Value);
                }

                // Only add these if they were really specified / known
                if (Alt != null) _imgTag.Alt(Alt);
                if (Class != null) _imgTag.Class(Class);
                if (Width != null) _imgTag.Width(Width);
                if (Height != null) _imgTag.Height(Height);

                return wrapLog("ok", _imgTag);
            }
        }
        private Img _imgTag;


        /// <inheritdoc />
        public string Alt => _alt.Get(() => Call.ImgAlt ?? Call.Field?.ImageDecoratorOrNull?.Description);
        private readonly PropertyToRetrieveOnce<string> _alt = new PropertyToRetrieveOnce<string>();


        /// <inheritdoc />
        public string Class => _imgClass.Get(ClassGenerator);
        private readonly PropertyToRetrieveOnce<string> _imgClass = new PropertyToRetrieveOnce<string>();
        private string ClassGenerator()
        {
            var wrapLog = Log.SafeCall<string>(ImgService.Debug);
            var part1 = Call.ImgClass;
            object attrClassObj = null;
            ThisResize.Recipe?.Attributes?.TryGetValue(Recipe.SpecialPropertyClass, out attrClassObj);
            var attrClass = attrClassObj as string;
            var hasOnAttrs = !string.IsNullOrWhiteSpace(attrClass);
            var hasOnImgClass = !string.IsNullOrWhiteSpace(Call.ImgClass);

            // Must use null if neither are useful
            if (!hasOnAttrs && !hasOnImgClass) return wrapLog("null/nothing", null);
            var result = part1 + (hasOnImgClass && hasOnAttrs ? " " : "") + attrClass;
            return wrapLog(result, result);
        }



        /// <inheritdoc />
        public bool ShowAll => ThisResize.ShowAll;

        /// <inheritdoc />
        public string SrcSet => _srcSet.Get(SrcSetGenerator);
        private readonly PropertyToRetrieveOnce<string> _srcSet = new PropertyToRetrieveOnce<string>();
        private string SrcSetGenerator()
        {
            var isEnabled = ImgService.Features.IsEnabled(FeaturesCatalog.ImageServiceMultipleSizes.NameId);
            var hasVariants = !string.IsNullOrWhiteSpace(ThisResize?.Recipe?.Variants);
            var wrapLog = Log.SafeCall<string>(ImgService.Debug, $"{nameof(isEnabled)}: {isEnabled}, {nameof(hasVariants)}: {hasVariants}");
            var result = isEnabled && hasVariants
                ? ImgLinker.SrcSet(Call.Link.Url, Settings, SrcSetType.Img, Call.Field)
                : null;
            return wrapLog(result, result);
        }



        /// <inheritdoc />
        public string Width => _width.Get(WidthGenerator);
        private readonly PropertyToRetrieveOnce<string> _width = new PropertyToRetrieveOnce<string>();
        private string WidthGenerator()
        {
            var setWidth = ThisResize.Recipe?.SetWidth;
            var wrapLog = Log.SafeCall<string>(ImgService.Debug, $"setWidth: {setWidth}, Width: {ThisResize.Width}");
            var result = setWidth == true && ThisResize.Width != 0
                ? ThisResize.Width.ToString()
                : null;
            return wrapLog(result, result);
        }



        /// <inheritdoc />
        public string Height => _height.Get(HeightGenerator);
        private readonly PropertyToRetrieveOnce<string> _height = new PropertyToRetrieveOnce<string>();
        private string HeightGenerator()
        {
            var setHeight = ThisResize.Recipe?.SetHeight;
            var wrapLog = Log.SafeCall<string>(ImgService.Debug, $"setHeight: {setHeight}, Height: {ThisResize.Height}");
            var result = setHeight == true && ThisResize.Height != 0
                ? ThisResize.Height.ToString()
                : null;
            return wrapLog(result, result);
        }


        public string Sizes => _sizes.Get(SizesGenerator);
        private readonly PropertyToRetrieveOnce<string> _sizes = new PropertyToRetrieveOnce<string>();
        private string SizesGenerator()
        {
            var wrapLog = Log.SafeCall<string>(ImgService.Debug);
            if (!ImgService.Features.IsEnabled(FeaturesCatalog.ImageServiceSetSizes.NameId))
                return wrapLog("disabled", null);
            var sizes = ThisResize?.Recipe?.Sizes;
            return wrapLog(sizes, sizes);
        }

        /// <inheritdoc />
        public string Url => ThisResize.Url;

    }
}
