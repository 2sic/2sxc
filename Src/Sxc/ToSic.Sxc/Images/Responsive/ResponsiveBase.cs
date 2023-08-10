using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Sxc.Data.Decorators;
using ToSic.Sxc.Web;
using static ToSic.Sxc.Configuration.Features.BuiltInFeatures;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public abstract class ResponsiveBase: HybridHtmlStringLog, IResponsiveImage
    {

        protected ResponsiveBase(ImageService imgService, ResponsiveParams callParams, ILog parentLog, string logName)
            : base(parentLog, $"Img.{logName}")
        {
            Call = callParams;
            ImgService = imgService;
            ImgLinker = imgService.ImgLinker;
        }
        protected ResponsiveParams Call { get; }
        protected readonly ImgResizeLinker ImgLinker;
        protected readonly ImageService ImgService;

        protected OneResize ThisResize => _thisResize.Get(() => { 
            var t = ImgLinker.ImageOnly(Call.Link.Url, Settings as ResizeSettings, Call.HasDecoOrNull);
            Log.A(ImgService.Debug, $"{nameof(ThisResize)}: " + t?.Dump());
            return t;
        });
        private readonly GetOnce<OneResize> _thisResize = new GetOnce<OneResize>();


        internal IResizeSettings Settings => Call.Settings;


        /// <summary>
        /// ToString must be specified by each implementation
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Tag.ToString();

        /// <inheritdoc />
        public virtual Img Img => _imgTag.Get(Log, l =>
        {
            var imgTag = Razor.Blade.Tag.Img().Src(Src);

            // Add all kind of attributes if specified
            var tag = ThisResize.Recipe;
            var dic = tag?.Attributes?
                .Where(pair => !Recipe.SpecialProperties.Contains(pair.Key))
                .ToDictionary(p => p.Key, p => p.Value);
            if (dic != null)
            {
                l.A(ImgService.Debug, "will add properties from attributes");
                foreach (var a in dic)
                    imgTag = imgTag.Attr(a.Key, a.Value);
            }

            // Only add these if they were really specified / known
            if (Alt != null) imgTag = imgTag.Alt(Alt);
            if (Class != null) imgTag = imgTag.Class(Class);
            if (Width != null) imgTag = imgTag.Width(Width);
            if (Height != null) imgTag = imgTag.Height(Height);

            return imgTag;
        }, enabled: ImgService.Debug);
        private readonly GetOnce<Img> _imgTag = new GetOnce<Img>();

        public IHtmlTag Tag => _tag.Get(GetTagWithToolbar);
        private readonly GetOnce<IHtmlTag> _tag = new GetOnce<IHtmlTag>();

        protected virtual IHtmlTag GetOutermostTag() => Img;

        private IHtmlTag GetTagWithToolbar()
        {
            var tag = GetOutermostTag();

            // fairly experimental, don't want things to break
            try
            {
                // attach edit if we are in edit-mode and the link was generated through a call
                if (ImgService.EditOrNull?.Enabled != true) return tag;
                if (Call.Field?.Parent == null) return tag;

                // Determine if this is an "own" adam file, because only field-owned files should allow config
                var isInSameEntity = Adam.Security.PathIsInItemAdam(Call.Field.Parent.Guid, "", Src);
                if (!isInSameEntity) return tag;

                // Check if it's not a demo-entity, in which case editing settings shouldn't happen
                if (Call.Field.Parent.Entity.DisableInlineEditSafe()) return tag;

                // var tlbUi = ImgService.ToolbarOrNull?.
                var toolbarConfig = ImgService.ToolbarOrNull?.Empty().Metadata(Call.Field).Settings(hover: "right-middle");
                var toolbar = ImgService.EditOrNull.TagToolbar(toolbar: toolbarConfig).ToString();
                tag.Attr(toolbar);
            }
            catch { /* ignore */ }

            return tag;
        }

        public string Description => Call.Description;

        /// <inheritdoc />
        public string Alt => _alt.Get(() =>
            // If alt is specified, it takes precedence - even if it's an empty string, because there must have been a reason for this
            Call.ImgAlt 
            // If we take the image description, empty does NOT take precedence, it will be treated as not-set
            ?? Description.NullIfNoValue()
            // If all else fails, take the fallback specified in the call
            ?? Call.ImgAltFallback
            );
        private readonly GetOnce<string> _alt = new GetOnce<string>();


        /// <inheritdoc />
        public string Class => _imgClass.Get(ClassGenerator);
        private readonly GetOnce<string> _imgClass = new GetOnce<string>();

        private string ClassGenerator() => Log.Func(() =>
        {
            var part1 = Call.ImgClass;
            object attrClass = null;
            ThisResize.Recipe?.Attributes?.TryGetValue(Recipe.SpecialPropertyClass, out attrClass);
            // var attrClass = attrClassObj;
            var hasOnAttrs = !string.IsNullOrWhiteSpace(attrClass?.ToString());
            var hasOnImgClass = !string.IsNullOrWhiteSpace(Call.ImgClass);

            // Must use null if neither are useful
            if (!hasOnAttrs && !hasOnImgClass) return (null, "null/nothing");
            var result = part1 + (hasOnImgClass && hasOnAttrs ? " " : "") + attrClass;
            return (result, "");
        }, enabled: ImgService.Debug);



        /// <inheritdoc />
        public bool ShowAll => ThisResize.ShowAll;

        /// <inheritdoc />
        public string SrcSet => _srcSet.Get(SrcSetGenerator);
        private readonly GetOnce<string> _srcSet = new GetOnce<string>();
        private string SrcSetGenerator()
        {
            var isEnabled = ImgService.Features.IsEnabled(ImageServiceMultipleSizes.NameId);
            var hasVariants = !string.IsNullOrWhiteSpace(ThisResize?.Recipe?.Variants);
            return Log.Func($"{nameof(isEnabled)}: {isEnabled}, {nameof(hasVariants)}: {hasVariants}",
                () => isEnabled && hasVariants
                    ? ImgLinker.SrcSet(Call.Link.Url, Settings as ResizeSettings, SrcSetType.Img, Call.HasDecoOrNull)
                    : null,
                enabled: ImgService.Debug);
        }



        /// <inheritdoc />
        public string Width => _width.Get(WidthGenerator);
        private readonly GetOnce<string> _width = new GetOnce<string>();
        private string WidthGenerator()
        {
            var setWidth = ThisResize.Recipe?.SetWidth;
            return Log.Func($"setWidth: {setWidth}, Width: {ThisResize.Width}",
                () => setWidth == true && ThisResize.Width != 0
                    ? ThisResize.Width.ToString()
                    : null,
                enabled: ImgService.Debug);
        }



        /// <inheritdoc />
        public string Height => _height.Get(HeightGenerator);
        private readonly GetOnce<string> _height = new GetOnce<string>();
        private string HeightGenerator()
        {
            var setHeight = ThisResize.Recipe?.SetHeight;
            return Log.Func($"setHeight: {setHeight}, Height: {ThisResize.Height}",
                () => setHeight == true && ThisResize.Height != 0
                    ? ThisResize.Height.ToString()
                    : null,
                enabled: ImgService.Debug);
        }


        public string Sizes => _sizes.Get(SizesGenerator);
        private readonly GetOnce<string> _sizes = new GetOnce<string>();

        private string SizesGenerator() => Log.Func(() =>
        {
            if (!ImgService.Features.IsEnabled(ImageServiceSetSizes.NameId))
                return (null, "disabled");
            var sizes = ThisResize.Recipe?.Sizes;
            return (sizes, "");
        }, enabled: ImgService.Debug);

        ///// <inheritdoc />
        //[PrivateApi]
        //public string Url => ThisResize.Url;

        public string Src => ThisResize.Url;

    }
}
