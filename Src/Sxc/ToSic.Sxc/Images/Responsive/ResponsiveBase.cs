using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Sxc.Web;
using static ToSic.Eav.Configuration.FeaturesBuiltIn;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public abstract class ResponsiveBase: HybridHtmlStringLog, IResponsiveImage
    {

        protected ResponsiveBase(ImageService imgService, ResponsiveParams callParams, string logName)
            : base($"Img.{logName}")
        {
            Call = callParams;
            ImgService = imgService;
            ImgLinker = imgService.ImgLinker;
        }
        protected ResponsiveParams Call { get; }
        protected readonly ImgResizeLinker ImgLinker;
        protected readonly ImageService ImgService;

        protected OneResize ThisResize => _thisResize.Get(() => { 
            var t = ImgLinker.ImageOnly(Call.Link.Url, Settings as ResizeSettings, Call.Field);
            Log.SafeAdd(ImgService.Debug, $"{nameof(ThisResize)}: " + t?.Dump());
            return t;
        });
        private readonly ValueGetOnce<OneResize> _thisResize = new ValueGetOnce<OneResize>();


        internal IResizeSettings Settings => Call.Settings;


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
                    .ToDictionary(p => p.Key, p => p.Value);
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
        private readonly ValueGetOnce<string> _alt = new ValueGetOnce<string>();


        /// <inheritdoc />
        public string Class => _imgClass.Get(ClassGenerator);
        private readonly ValueGetOnce<string> _imgClass = new ValueGetOnce<string>();
        private string ClassGenerator()
        {
            var wrapLog = Log.SafeCall<string>(ImgService.Debug);
            var part1 = Call.ImgClass;
            string attrClass = null;
            ThisResize.Recipe?.Attributes?.TryGetValue(Recipe.SpecialPropertyClass, out attrClass);
            // var attrClass = attrClassObj;
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
        private readonly ValueGetOnce<string> _srcSet = new ValueGetOnce<string>();
        private string SrcSetGenerator()
        {
            var isEnabled = ImgService.Features.IsEnabled(ImageServiceMultipleSizes.NameId);
            var hasVariants = !string.IsNullOrWhiteSpace(ThisResize?.Recipe?.Variants);
            var wrapLog = Log.SafeCall<string>(ImgService.Debug, $"{nameof(isEnabled)}: {isEnabled}, {nameof(hasVariants)}: {hasVariants}");
            var result = isEnabled && hasVariants
                ? ImgLinker.SrcSet(Call.Link.Url, Settings as ResizeSettings, SrcSetType.Img, Call.Field)
                : null;
            return wrapLog(result, result);
        }



        /// <inheritdoc />
        public string Width => _width.Get(WidthGenerator);
        private readonly ValueGetOnce<string> _width = new ValueGetOnce<string>();
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
        private readonly ValueGetOnce<string> _height = new ValueGetOnce<string>();
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
        private readonly ValueGetOnce<string> _sizes = new ValueGetOnce<string>();
        private string SizesGenerator()
        {
            var wrapLog = Log.SafeCall<string>(ImgService.Debug);
            if (!ImgService.Features.IsEnabled(ImageServiceSetSizes.NameId))
                return wrapLog("disabled", null);
            var sizes = ThisResize.Recipe?.Sizes;
            return wrapLog(sizes, sizes);
        }

        /// <inheritdoc />
        public string Url => ThisResize.Url;

    }
}
