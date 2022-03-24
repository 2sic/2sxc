using System.Linq;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Sxc.Plumbing;
using ToSic.Sxc.Web;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public abstract class ResponsiveBase: HybridHtmlString
    {

        protected ResponsiveBase(ImageService imgService, ResponsiveParams responsiveParams)
        {
            Call = responsiveParams;
            ImgService = imgService;
            ImgLinker = imgService.ImgLinker;
            Settings = PrepareResizeSettings(Call.Settings, Call.Factor, Call.Recipe);

        }
        protected ResponsiveParams Call { get; }
        protected readonly ImgResizeLinker ImgLinker;
        protected readonly ImageService ImgService;

        public string Url => ThisResize.Url;

        protected OneResize ThisResize => _thisResize ?? (_thisResize = ImgLinker.ImageOnly(Call.Link.Url, Settings, Call.Field));
        private OneResize _thisResize;

        internal ResizeSettings Settings { get; }


        protected ResizeSettings PrepareResizeSettings(object settings, object factor, AdvancedSettings mrs)
        {
            // 1. Prepare Settings
            if (settings is ResizeSettings resSettings)
            {
                // If we have a modified factor, make sure we have that (this will copy the settings)
                var newFactor = ParseObject.DoubleOrNullWithCalculation(factor);
                resSettings = new ResizeSettings(resSettings, factor: newFactor ?? resSettings.Factor, mrs);
            }
            else
                resSettings = ImgLinker.ResizeParamMerger.BuildResizeSettings(settings, factor: factor, /*allowMulti: true,*/ advanced: mrs);

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

                // Add all kind of attributes if specified
                var tag = ThisResize.TagEnhancements;
                var dic = tag?.Attributes?
                    .Where(pair => !Recipe.SpecialProperties.Contains(pair.Key))
                    .ToDictionary(p => p.Key, p => p.Value); ;
                if (dic != null)
                    foreach (var a in dic)
                        _imgTag.Attr(a.Key, a.Value);

                // Only add these if they were really specified
                var imgAlt = Call.ImgAlt ?? Call.Field?.ImageDecoratorOrNull()?.Description;
                if (imgAlt != null) _imgTag.Alt(imgAlt);
                
                // Note that adding a class will keep previous class added
                if (Call.ImgClass != null) _imgTag.Class(Call.ImgClass);

                // Optionally set width and height if known
                if (tag?.SetWidth == true && ThisResize.Width != 0) _imgTag.Width(ThisResize.Width);
                if (tag?.SetHeight == true && ThisResize.Height != 0) _imgTag.Height(ThisResize.Height);

                return _imgTag;
            }
        }

        private Img _imgTag;

    }
}
