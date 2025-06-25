using System.Runtime.CompilerServices;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using ToSic.Sxc.Data.Sys.Decorators;
using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.Configuration;
using ToSic.Sxc.Web.Internal.PageFeatures;
using ToSic.Sxc.Web.Sys.Html;
using ToSic.Sys.Utils;
using static System.StringComparer;

namespace ToSic.Sxc.Images.Internal;

/// <remarks>
/// Must be public, otherwise it breaks in dynamic use :(
/// </remarks>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract record ResponsiveBase: HybridHtmlStringLog, IResponsiveImage
{
    /// <summary>
    /// Constructor is internal so 1. it can't be created from outside and 2. it has parameters which are internal.
    /// </summary>
    internal ResponsiveBase(ImageService imgService, IPageService pageService, ResponsiveSpecs specs, ILog parentLog, string logName)
        : base(parentLog, $"Img.{logName}")
    {
        Specs = specs;
        PageService = pageService;
        ImgService = imgService;
    }

    internal ResponsiveSpecsOfTarget Target => Specs.Target;

    internal ResponsiveSpecs Specs { get; init; }
    internal ImageService ImgService { get; init; }
    protected IPageService PageService { get; init; }
    internal TweakMedia Tweaker => Specs.Tweaker;
    internal ResizeSettings Settings => Tweaker.ResizeSettings;

    private OneResize ThisResize => _thisResize.Get(() => { 
        var t = ImgService.ImgLinker.ImageOnly(Target.Link.Url, Settings, Target.HasMdOrNull);
        Log.A(ImgService.Debug, $"{nameof(ThisResize)}: " + t.Dump());
        return t;
    })!;
    private readonly GetOnce<OneResize> _thisResize = new();




    /// <summary>
    /// ToString must be specified by each implementation
    /// </summary>
    /// <returns></returns>
    protected override string ToHtmlString() => Tag.ToString();

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public virtual Img Img => field ??= GetImg();

    private Img GetImg()
    {
        var l = Log.Fn<Img>(enabled: ImgService.Debug);
        var imgTag = ToSic.Razor.Blade.Tag.Img().Src(Src);

        // Add all kind of attributes if specified
        var imgSpecs = Tweaker.Img;
        imgTag = AddAttributes(imgTag, imgSpecs.Attributes);
        imgTag = AddAttributes(imgTag, ThisResize.Recipe?.Attributes, imgSpecs.Attributes?.Keys);

        // Only add Alt, Class etc. if they were really specified / known
        if (Alt != null)
            imgTag = imgTag.Alt(Alt);
        if (Class != null)
            imgTag = imgTag.Class(Class);
        if (TryGetAttribute(imgSpecs.Attributes, Recipe.SpecialPropertyStyle, out var style))
            imgTag = imgTag.Style(style);
        if (TryGetAttribute(ThisResize.Recipe?.Attributes, Recipe.SpecialPropertyStyle, out style))
            imgTag = imgTag.Style(style);
        if (Width != null)
            imgTag = imgTag.Width(Width);
        if (Height != null)
            imgTag = imgTag.Height(Height);

        // Add lightbox if configured & enabled on the specific image...
        var enabled = Tweaker.VDec.LightboxIsEnabled
                      ?? Target.ImgDecoratorOrNull?.LightboxIsEnabled
                      ?? Target.FieldImgDecoratorOrNull?.LightboxIsEnabled
                      ?? false;

        if (enabled)
            imgTag = AddLightbox(
                imgTag,
                imageGroup: Tweaker.VDec.LightboxGroup.NullIfNoValue()
                            ?? Target.ImgDecoratorOrNull?.LightboxGroup.NullIfNoValue()
                            ?? Target.FieldImgDecoratorOrNull?.LightboxGroup,
                description: Tweaker.VDec.DescriptionExtended
                             ?? Target.ImgDecoratorOrNull?.DescriptionExtended
            );

        // #alwaysOnImg
        //if (Params.Toolbar as string == "img")
        {
            var tlb = ToolbarOrNull(/*true*/);
            if (tlb != null)
                imgTag = imgTag.Attr(tlb);
        }

        return l.Return(imgTag);
    }


    /// <summary>
    /// Mark the image for lightbox use, and possibly give it the attributes like
    /// - data-title="My caption"
    /// - data-alt="My alt text"
    /// - large image
    /// </summary>
    /// <param name="original"></param>
    /// <param name="imageGroup"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    private Img AddLightbox(Img original, string? imageGroup, string? description)
    {
        var l = Log.Fn<Img>();

        // Mark image for lightbox use, different html for single image or group
        var img = imageGroup.HasValue()
            ? original.Attr(LightboxHelpers.AttributeGroup, imageGroup)
            : original.Attr(LightboxHelpers.Attribute);

        var lsSettings = ImgService.SettingsInternal(LightboxHelpers.SettingsName);
        var lsUrl = ImgService.ImgLinker.ImageOnly(Target.Link?.Url, settings: lsSettings, Target.HasMdOrNull).Url;
        
        // Add Lightbox caption and src
        var caption = Alt + description;
        img = img
            .Attr("data-src", lsUrl)
            .Attr("data-caption", caption);

        // 2. Turn on lightbox feature of 2sxc
        // ...make sure it will also load the activation JS
        // Note: it would be better to just activate "lightbox", but ATM the features don't support finding dependent features from WebResources
        PageService.Activate(SxcPageFeatures.TurnOn.NameId, /*SxcPageFeatures.Lightbox.NameId,*/ SxcPageFeatures.WebResourceFancybox4.NameId);
        PageService.TurnOn(LightboxHelpers.JsCall, noDuplicates: true, args: LightboxHelpers.CreateArgs(imageGroup));

        return l.Return(img, "");
    }

    [PrivateApi]
    protected TImg AddAttributes<TImg>(TImg imgTag, IDictionary<string, object?>? addAttributes, IEnumerable<string>? skip = default) where TImg : Tag<TImg>
    {
        var l = Log.Fn<TImg>();
        if (addAttributes == null || addAttributes.Count == 0)
            return l.Return(imgTag, "nothing to add");

        string[] exclude = [..Recipe.SpecialProperties, ..skip ?? [] ];

        var dic = addAttributes
            .Where(pair => !exclude.Contains(pair.Key, comparer: InvariantCultureIgnoreCase))
            .ToDictionary(p => p.Key, p => p.Value);
        if (dic.Count == 0)
            return l.Return(imgTag, "only special props");

        l.A(ImgService.Debug, "will add properties from attributes");
        foreach (var a in dic)
            imgTag = imgTag.Attr(a.Key, a.Value);

        return l.Return(imgTag, "added");
    }


    public IHtmlTag Tag => _tag.Get(GetOutermostTag)!;
    private readonly GetOnce<IHtmlTag> _tag = new();

    protected virtual IHtmlTag GetOutermostTag() => Img;


    /// <summary>
    /// Get the toolbar - or null, based on
    /// - various conditions if toolbars are available
    /// - the question if it is being retrieved for the IMG tag or not.
    /// </summary>
    /// <returns></returns>
    private IToolbarBuilder? ToolbarOrNull()
    {
        // attach edit if we are in edit-mode and the link was generated through a call
        if (ImgService.EditOrNull?.Enabled != true)
            return null;
        if (Target.FieldOrNull?.Parent == null)
            return null;

        // Check if it's not a demo-entity, in which case editing settings shouldn't happen
        if (Target.FieldOrNull.Parent.Entity.DisableInlineEditSafe())
            return null;

        // Get toolbar - if it's null (basically when the ImageService fails) stop here
        return Toolbar();
    }

    #region Toolbar

    /// <inheritdoc />
    // note: it's a method but ATM always returns the cached toolbar
    // still implemented as a method, so we could add future parameters if necessary
    public IToolbarBuilder? Toolbar() => _toolbar.Get(() => new ResponsiveToolbarBuilder(Log).Toolbar(ImgService, Target, Tweaker, Settings, Src));

    private readonly GetOnce<IToolbarBuilder?> _toolbar = new();

    #endregion


    /// <inheritdoc />
    public string? Description => _description.Get(() => Target.ImgDecoratorOrNull?.Description);
    private readonly GetOnce<string?> _description = new();

    /// <inheritdoc />
    public string? DescriptionExtended => _descriptionDet.Get(() => Target.ImgDecoratorOrNull?.DescriptionExtended);
    private readonly GetOnce<string?> _descriptionDet = new();

    /// <inheritdoc />
    public string? Alt => _alt.Get(() =>
        // If alt is specified, it takes precedence - even if it's an empty string, because there must have been a reason for this
        Tweaker.Img.Alt
        // If we take the image description, empty does NOT take precedence, it will be treated as not-set
        ?? Description.NullIfNoValue()
        // If all else fails, take the fallback specified in the call - IF it's allowed
        ?? (Target.ImgDecoratorOrNull?.SkipFallbackTitle ?? false ? null : Tweaker.Img.AltFallback)
    );
    private readonly GetOnce<string?> _alt = new();


    /// <inheritdoc />
    public string? Class => _imgClass.Get(() => StyleOrClassGenerator(Tweaker.Img.Class, Recipe.SpecialPropertyClass));
    private readonly GetOnce<string?> _imgClass = new();

    private string? StyleOrClassGenerator(string? codePart, string key)
    {
        var l = (ImgService.Debug ? Log : null).Fn<string?>();
        var hasOnImgClass = codePart.HasValue();
        var hasOnAttrs = TryGetAttribute(ThisResize.Recipe?.Attributes, key, out var attrValue);

        return hasOnImgClass switch
        {
            // Must use null if neither are useful
            false when !hasOnAttrs => l.ReturnNull("null/nothing"),
            true when hasOnAttrs => l.Return($"{codePart} {attrValue}", "both"),
            true => l.Return(codePart, "code only"),
            _ => l.Return(attrValue, "attr only")
        };
    }

    [PrivateApi]
    protected bool TryGetAttribute(IDictionary<string, object?>? attribs, string key, out string? value)
    {
        if (attribs == null)
        {
            value = null;
            return false;
        }
        var found = attribs.TryGetValue(key, out var attrValue);
        value = attrValue?.ToString();
        return found && value.HasValue();
    }


    /// <inheritdoc />
    public bool ShowAll => ThisResize.ShowAll;

    /// <inheritdoc />
    public string? SrcSet => _srcSet.Get(SrcSetGenerator);
    private readonly GetOnce<string?> _srcSet = new();
    private string? SrcSetGenerator()
    {
        var isEnabled = ImgService.Features.IsEnabled(SxcFeatures.ImageServiceMultipleSizes.NameId);
        var hasVariants = (ThisResize?.Recipe?.Variants).HasValue();
        var l = (ImgService.Debug ? Log : null).Fn<string?>($"{nameof(isEnabled)}: {isEnabled}, {nameof(hasVariants)}: {hasVariants}");
        return isEnabled && hasVariants
            ? l.Return(ImgService.ImgLinker.SrcSet(Target.Link.Url!, Settings, SrcSetType.Img,
                Target.HasMdOrNull))
            : l.ReturnNull();
    }



    /// <inheritdoc />
    public string? Width => _width.Get(() => UseIfActive(ThisResize.Recipe?.SetWidth, ThisResize.Width));
    private readonly GetOnce<string?> _width = new();

    /// <inheritdoc />
    public string? Height => _height.Get(() => UseIfActive(ThisResize.Recipe?.SetHeight, ThisResize.Height));
    private readonly GetOnce<string?> _height = new();

    /// <inheritdoc />
    public string? Sizes => _sizes.Get(() => UseIfActive(ImgService.Features.IsEnabled(SxcFeatures.ImageServiceSetSizes.NameId), ThisResize.Recipe?.Sizes));
    private readonly GetOnce<string?> _sizes = new();

    private string? UseIfActive<T>(bool? active, T value, [CallerMemberName] string? name = default)
    {
        var l = (ImgService.Debug ? Log : null).Fn<string>($"{name}: active: {active}; value: {value}");
        return active == true && value.IsNotDefault()
            ? l.ReturnAndLog($"{value}")
            : l.ReturnNull("disabled");
    }


    /// <inheritdoc />
    public string Src => ThisResize.Url ?? "";

}