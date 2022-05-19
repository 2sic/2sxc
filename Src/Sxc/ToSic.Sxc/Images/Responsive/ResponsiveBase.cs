using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using ToSic.Sxc.Web;
using static ToSic.Sxc.Configuration.Features.BuiltInFeatures;

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
            Log.A(ImgService.Debug, $"{nameof(ThisResize)}: " + t?.Dump());
            return t;
        });
        private readonly ValueGetOnce<OneResize> _thisResize = new ValueGetOnce<OneResize>();


        internal IResizeSettings Settings => Call.Settings;


        /// <summary>
        /// ToString must be specified by each implementation
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Tag.ToString();

        /// <inheritdoc />
        public virtual Img Img
        {
            get
            {
                if (_imgTag != null) return _imgTag;

                var wrapLog = Log.Call2<Img>(ImgService.Debug);
                _imgTag = Razor.Blade.Tag.Img().Src(Src);

                // Add all kind of attributes if specified
                var tag = ThisResize.Recipe;
                var dic = tag?.Attributes?
                    .Where(pair => !Recipe.SpecialProperties.Contains(pair.Key))
                    .ToDictionary(p => p.Key, p => p.Value);
                if (dic != null)
                {
                    Log.A(ImgService.Debug, "will add properties from attributes");
                    foreach (var a in dic)
                        _imgTag.Attr(a.Key, a.Value);
                }

                // Only add these if they were really specified / known
                if (Alt != null) _imgTag.Alt(Alt);
                if (Class != null) _imgTag.Class(Class);
                if (Width != null) _imgTag.Width(Width);
                if (Height != null) _imgTag.Height(Height);

                return wrapLog.Return(_imgTag, "ok");
            }
        }
        private Img _imgTag;

        public ITag Tag => _tag.Get(GetTagWithToolbar);
        private readonly ValueGetOnce<ITag> _tag = new ValueGetOnce<ITag>();

        protected virtual ITag GetOutermostTag() => Img;

        private ITag GetTagWithToolbar()
        {
            var tag = GetOutermostTag();

            // fairly experimental, don't want things to break
            try
            {
                // attach edit if we are in edit-mode and the link was generated through a call
                if (ImgService.EditOrNull?.Enabled != true) return tag;
                if (Call.Field?.Parent == null) return tag;

                // TODO: Determine if this is an "own" adam file
                var isInSameEntity = Adam.Security.PathIsInItemAdam(Call.Field.Parent.EntityGuid, "", Src);
                if (!isInSameEntity) return tag;

                var toolbarConfig = ImgService.ToolbarOrNull?.Empty().Image(Call.Field);
                var toolbar = ImgService.EditOrNull.TagToolbar(toolbar: toolbarConfig).ToString();
                tag.TagAttributes.Add(toolbar);
            }
            catch { /* ignore */ }

            return tag;
        }


        /// <inheritdoc />
        public string Alt => _alt.Get(() => Call.ImgAlt ?? Call.Field?.ImageDecoratorOrNull?.Description);
        private readonly ValueGetOnce<string> _alt = new ValueGetOnce<string>();


        /// <inheritdoc />
        public string Class => _imgClass.Get(ClassGenerator);
        private readonly ValueGetOnce<string> _imgClass = new ValueGetOnce<string>();
        private string ClassGenerator()
        {
            var wrapLog = Log.Call2<string>(ImgService.Debug);
            var part1 = Call.ImgClass;
            string attrClass = null;
            ThisResize.Recipe?.Attributes?.TryGetValue(Recipe.SpecialPropertyClass, out attrClass);
            // var attrClass = attrClassObj;
            var hasOnAttrs = !string.IsNullOrWhiteSpace(attrClass);
            var hasOnImgClass = !string.IsNullOrWhiteSpace(Call.ImgClass);

            // Must use null if neither are useful
            if (!hasOnAttrs && !hasOnImgClass) return wrapLog.ReturnNull("null/nothing");
            var result = part1 + (hasOnImgClass && hasOnAttrs ? " " : "") + attrClass;
            return wrapLog.ReturnAndLog(result);
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
            var wrapLog = Log.Call2<string>(ImgService.Debug, $"{nameof(isEnabled)}: {isEnabled}, {nameof(hasVariants)}: {hasVariants}");
            var result = isEnabled && hasVariants
                ? ImgLinker.SrcSet(Call.Link.Url, Settings as ResizeSettings, SrcSetType.Img, Call.Field)
                : null;
            return wrapLog.ReturnAndLog(result);
        }



        /// <inheritdoc />
        public string Width => _width.Get(WidthGenerator);
        private readonly ValueGetOnce<string> _width = new ValueGetOnce<string>();
        private string WidthGenerator()
        {
            var setWidth = ThisResize.Recipe?.SetWidth;
            var wrapLog = Log.Call2<string>(ImgService.Debug, $"setWidth: {setWidth}, Width: {ThisResize.Width}");
            var result = setWidth == true && ThisResize.Width != 0
                ? ThisResize.Width.ToString()
                : null;
            return wrapLog.ReturnAndLog(result);
        }



        /// <inheritdoc />
        public string Height => _height.Get(HeightGenerator);
        private readonly ValueGetOnce<string> _height = new ValueGetOnce<string>();
        private string HeightGenerator()
        {
            var setHeight = ThisResize.Recipe?.SetHeight;
            var wrapLog = Log.Call2<string>(ImgService.Debug, $"setHeight: {setHeight}, Height: {ThisResize.Height}");
            var result = setHeight == true && ThisResize.Height != 0
                ? ThisResize.Height.ToString()
                : null;
            return wrapLog.ReturnAndLog(result);
        }


        public string Sizes => _sizes.Get(SizesGenerator);
        private readonly ValueGetOnce<string> _sizes = new ValueGetOnce<string>();
        private string SizesGenerator()
        {
            var wrapLog = Log.Call2<string>(ImgService.Debug);
            if (!ImgService.Features.IsEnabled(ImageServiceSetSizes.NameId))
                return wrapLog.ReturnNull("disabled");
            var sizes = ThisResize.Recipe?.Sizes;
            return wrapLog.ReturnAndLog(sizes);
        }

        ///// <inheritdoc />
        //[PrivateApi]
        //public string Url => ThisResize.Url;

        public string Src => ThisResize.Url;

    }
}
