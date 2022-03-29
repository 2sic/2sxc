using System.Linq;
using ToSic.Eav.Configuration;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Sxc.Plumbing;
using ToSic.Sxc.Web;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public abstract class ResponsiveBase: HybridHtmlString, IResponsiveImage
    {

        protected ResponsiveBase(ImageService imgService, ResponsiveParams responsiveParams)
        {
            Call = responsiveParams;
            ImgService = imgService;
            ImgLinker = imgService.ImgLinker;
            Settings = PrepareResizeSettings(Call.Settings, Call.Factor, Call.Advanced);
        }
        protected ResponsiveParams Call { get; }
        protected readonly ImgResizeLinker ImgLinker;
        protected readonly ImageService ImgService;

        protected OneResize ThisResize => _thisResize ?? (_thisResize = ImgLinker.ImageOnly(Call.Link.Url, Settings, Call.Field));
        private OneResize _thisResize;

        internal ResizeSettings Settings { get; }


        protected ResizeSettings PrepareResizeSettings(object settings, object factor, AdvancedSettings advanced)
        {
            // 1. Prepare Settings
            if (settings is ResizeSettings resSettings)
            {
                // If we have a modified factor, make sure we have that (this will copy the settings)
                var newFactor = ParseObject.DoubleOrNullWithCalculation(factor);
                resSettings = new ResizeSettings(resSettings, factor: newFactor ?? resSettings.Factor, advanced);
            }
            else
                resSettings = ImgLinker.ResizeParamMerger.BuildResizeSettings(settings, factor: factor, advanced: advanced);

            return resSettings;
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

                _imgTag = Tag.Img().Src(Url);

                // Add all kind of attributes if specified
                var tag = ThisResize.TagEnhancements;
                var dic = tag?.Attributes?
                    .Where(pair => !Recipe.SpecialProperties.Contains(pair.Key))
                    .ToDictionary(p => p.Key, p => p.Value); ;
                if (dic != null)
                    foreach (var a in dic)
                        _imgTag.Attr(a.Key, a.Value);

                // Only add these if they were really specified / known
                if (Alt != null) _imgTag.Alt(Alt);
                if (Class != null) _imgTag.Class(Class);
                if (Width != null) _imgTag.Width(Width);
                if (Height != null) _imgTag.Height(Height);

                return _imgTag;
            }
        }
        private Img _imgTag;


        /// <inheritdoc />
        public string Alt => _alt.Get(() => Call.ImgAlt ?? Call.Field?.ImageDecoratorOrNull?.Description);
        private readonly PropertyToRetrieveOnce<string> _alt = new PropertyToRetrieveOnce<string>();


        /// <inheritdoc />
        public string Class => _imgClass.Get(() =>
        {
            var part1 = Call.ImgClass;
            object attrClassObj = null;
            ThisResize.TagEnhancements?.Attributes?.TryGetValue(Recipe.SpecialPropertyClass, out attrClassObj);
            var attrClass = attrClassObj as string;
            var hasOnAttrs = !string.IsNullOrWhiteSpace(attrClass);
            var hasOnImgClass = !string.IsNullOrWhiteSpace(Call.ImgClass);
            
            // Must use null if neither are useful
            if (!hasOnAttrs && !hasOnImgClass) return null;
            return part1 + (hasOnImgClass && hasOnAttrs ? " " : "") + attrClass;
        });
        private readonly PropertyToRetrieveOnce<string> _imgClass = new PropertyToRetrieveOnce<string>();


        /// <inheritdoc />
        public bool ShowAll => ThisResize.ShowAll;

        /// <inheritdoc />
        public string SrcSet => _srcSet.Get(() =>
            ImgService.Features.IsEnabled(FeaturesCatalog.ImageServiceMultipleSizes.NameId)
                ? string.IsNullOrWhiteSpace(ThisResize?.TagEnhancements?.Variants)
                    ? null
                    : ImgLinker.SrcSet(Call.Link.Url, Settings, SrcSetType.Img, Call.Field)
                : null);
        private readonly PropertyToRetrieveOnce<string> _srcSet = new PropertyToRetrieveOnce<string>();


        /// <inheritdoc />
        public string Width => _width.Get(() =>
            ThisResize.TagEnhancements?.SetWidth == true && ThisResize.Width != 0
                ? ThisResize.Width.ToString()
                : null);
        private readonly PropertyToRetrieveOnce<string> _width = new PropertyToRetrieveOnce<string>();


        /// <inheritdoc />
        public string Height => _height.Get(() =>
            ThisResize.TagEnhancements?.SetHeight == true && ThisResize.Height != 0
                ? ThisResize.Height.ToString()
                : null);
        private readonly PropertyToRetrieveOnce<string> _height = new PropertyToRetrieveOnce<string>();

        /// <inheritdoc />
        public string Url => ThisResize.Url;

    }
}
