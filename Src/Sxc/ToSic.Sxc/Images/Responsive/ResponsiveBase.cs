using System.Linq;
using System.Runtime.CompilerServices;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Sxc.Data.Decorators;
using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Web;
using static ToSic.Sxc.Configuration.Features.BuiltInFeatures;
using static ToSic.Sxc.Images.ImageDecorator;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public abstract class ResponsiveBase: HybridHtmlStringLog, IResponsiveImage
    {

        protected ResponsiveBase(ImageService imgService, ResponsiveParams callParams, ILog parentLog, string logName)
            : base(parentLog, $"Img.{logName}")
        {
            Params = callParams;
            ImgService = imgService;
            ImgLinker = imgService.ImgLinker;
        }
        protected ResponsiveParams Params { get; }
        protected readonly ImgResizeLinker ImgLinker;
        protected readonly ImageService ImgService;

        protected OneResize ThisResize => _thisResize.Get(() => { 
            var t = ImgLinker.ImageOnly(Params.Link.Url, Settings as ResizeSettings, Params.HasMetadataOrNull);
            Log.A(ImgService.Debug, $"{nameof(ThisResize)}: " + t?.Dump());
            return t;
        });
        private readonly GetOnce<OneResize> _thisResize = new GetOnce<OneResize>();


        internal IResizeSettings Settings => Params.Settings;


        /// <summary>
        /// ToString must be specified by each implementation
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Tag.ToString();

        /// <inheritdoc />
        public virtual Img Img => _imgTag.GetL(Log, l =>
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
                if (Params.Field?.Parent == null) return tag;

                // Check if it's not a demo-entity, in which case editing settings shouldn't happen
                if (Params.Field.Parent.Entity.DisableInlineEditSafe()) return tag;

                // Get toolbar - if it's null (basically when the ImageService fails) stop here
                var toolbar = Toolbar();
                if (toolbar != null) tag.Attr(toolbar);
            }
            catch { /* ignore */ }

            return tag;
        }

        /// <inheritdoc />
        public IToolbarBuilder Toolbar() => _toolbar.Get(() =>
        {
            if (Params.Toolbar is bool toggle && !toggle) return null;
            if (Params.Toolbar is IToolbarBuilder customToolbar) return customToolbar;

            // If we're creating an image for a string value, it won't have a field or parent.
            if (Params.Field?.Parent == null || Params.HasMetadataOrNull == null) return null;

            // Determine if this is an "own" adam file, because only field-owned files should allow config
            var isInSameEntity = Adam.Security.PathIsInItemAdam(Params.Field.Parent.Guid, "", Src);

            // Construct the toolbar; in edge cases the toolbar service could be missing
            var imgTlb = ImgService.ToolbarOrNull?.Empty().Settings(
                hover: "right-middle",
                // Delay show of toolbar if it's a shared image, as it shouldn't be used much
                ui: isInSameEntity ? null : "delayShow=1000"
            );

            // Try to add the metadata button (or just null if not available)
            imgTlb = imgTlb?.Metadata(Params.HasMetadataOrNull,
                tweak: btn =>
                {
                    btn = btn.Tooltip($"{ToolbarConstants.ToolbarLabelPrefix}MetadataImage");
                    return isInSameEntity ? btn : btn.FormParameters(ShowWarningGlobalFile, true);
                });

            return imgTlb;
        });

        private readonly GetOnce<IToolbarBuilder> _toolbar = new GetOnce<IToolbarBuilder>();

        public string Description => _description.Get(() => Params.Field?.ImageDecoratorOrNull?.Description);
        private readonly GetOnce<string> _description = new GetOnce<string>();

        public string DescriptionExtended => _descriptionDet.Get(() => Params.Field?.ImageDecoratorOrNull?.DescriptionExtended);
        private readonly GetOnce<string> _descriptionDet = new GetOnce<string>();

        /// <inheritdoc />
        public string Alt => _alt.Get(() =>
            // If alt is specified, it takes precedence - even if it's an empty string, because there must have been a reason for this
            Params.ImgAlt 
            // If we take the image description, empty does NOT take precedence, it will be treated as not-set
            ?? Description.NullIfNoValue()
            // If all else fails, take the fallback specified in the call - IF it's allowed
            ?? (Params.Field?.ImageDecoratorOrNull?.SkipFallbackTitle ?? false ? null : Params.ImgAltFallback)
            );
        private readonly GetOnce<string> _alt = new GetOnce<string>();


        /// <inheritdoc />
        public string Class => _imgClass.Get(ClassGenerator);
        private readonly GetOnce<string> _imgClass = new GetOnce<string>();

        private string ClassGenerator() => Log.Func(() =>
        {
            var part1 = Params.ImgClass;
            object attrClass = null;
            ThisResize.Recipe?.Attributes?.TryGetValue(Recipe.SpecialPropertyClass, out attrClass);
            // var attrClass = attrClassObj;
            var hasOnAttrs = !string.IsNullOrWhiteSpace(attrClass?.ToString());
            var hasOnImgClass = !string.IsNullOrWhiteSpace(Params.ImgClass);

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
            var hasVariants = (ThisResize?.Recipe?.Variants).HasValue();
            var l = (ImgService.Debug ? Log : null).Fn<string>($"{nameof(isEnabled)}: {isEnabled}, {nameof(hasVariants)}: {hasVariants}");
            return isEnabled && hasVariants
                ? l.Return(ImgLinker.SrcSet(Params.Link.Url, Settings as ResizeSettings, SrcSetType.Img,
                    Params.HasMetadataOrNull))
                : l.ReturnNull();
        }



        /// <inheritdoc />
        public string Width => _width.Get(() => UseIfActive(ThisResize.Recipe?.SetWidth, ThisResize.Width));
        private readonly GetOnce<string> _width = new GetOnce<string>();

        /// <inheritdoc />
        public string Height => _height.Get(() => UseIfActive(ThisResize.Recipe?.SetHeight, ThisResize.Height));
        private readonly GetOnce<string> _height = new GetOnce<string>();

        /// <inheritdoc />
        public string Sizes => _sizes.Get(() => UseIfActive(ImgService.Features.IsEnabled(ImageServiceSetSizes.NameId), ThisResize.Recipe?.Sizes));
        private readonly GetOnce<string> _sizes = new GetOnce<string>();

        private string UseIfActive<T>(bool? active, T value, [CallerMemberName] string name = default)
        {
            var l = (ImgService.Debug ? Log : null).Fn<string>($"{name}: active: {active}; value: {value}");
            return active == true && value.IsNotDefault()
                ? l.ReturnAndLog($"{value}")
                : l.ReturnNull("disabled");
        }


        /// <inheritdoc />
        public string Src => ThisResize.Url;

    }
}
