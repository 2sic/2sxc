using System.Runtime.CompilerServices;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Data.Internal.Decorators;
using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Edit.Toolbar.Internal;
using ToSic.Sxc.Web.Internal;
using static System.StringComparer;
using static ToSic.Sxc.Configuration.Internal.SxcFeatures;
using static ToSic.Sxc.Images.Internal.ImageDecorator;

namespace ToSic.Sxc.Images.Internal;

/// <remarks>
/// Must be public, otherwise it breaks in dynamic use :(
/// </remarks>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class ResponsiveBase: HybridHtmlStringLog, IResponsiveImage
{

    internal ResponsiveBase(ImageService imgService, ResponsiveParams callParams, ILog parentLog, string logName)
        : base(parentLog, $"Img.{logName}")
    {
        Params = callParams;
        ImgService = imgService;
        ImgLinker = imgService.ImgLinker;
    }
    internal ResponsiveParams Params { get; }
    protected readonly ImgResizeLinker ImgLinker;
    internal readonly ImageService ImgService;

    private OneResize ThisResize => _thisResize.Get(() => { 
        var t = ImgLinker.ImageOnly(Params.Link.Url, Settings as ResizeSettings, Params.HasMetadataOrNull);
        Log.A(ImgService.Debug, $"{nameof(ThisResize)}: " + t?.Dump());
        return t;
    });
    private readonly GetOnce<OneResize> _thisResize = new();


    internal IResizeSettings Settings => Params.Settings;


    /// <summary>
    /// ToString must be specified by each implementation
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Tag.ToString();

    /// <inheritdoc />
    public virtual Img Img => _imgTag.GetL(Log, l =>
    {
        var imgTag = ToSic.Razor.Blade.Tag.Img().Src(Src);

        // Add all kind of attributes if specified
        imgTag = AddAttributes(imgTag, Params.ImgAttributes);
        imgTag = AddAttributes(imgTag, ThisResize.Recipe?.Attributes);

        // Only add these if they were really specified / known
        if (Alt != null) imgTag = imgTag.Alt(Alt);
        if (Class != null) imgTag = imgTag.Class(Class);
        if (TryGetAttribute(Params.ImgAttributes, Recipe.SpecialPropertyStyle, out var style))
            imgTag = imgTag.Style(style);
        if (TryGetAttribute(ThisResize.Recipe?.Attributes, Recipe.SpecialPropertyStyle, out style)) 
            imgTag = imgTag.Style(style);
        if (Width != null) imgTag = imgTag.Width(Width);
        if (Height != null) imgTag = imgTag.Height(Height);

        return imgTag;
    }, enabled: ImgService.Debug);

    [PrivateApi]
    protected TImg AddAttributes<TImg>(TImg imgTag, IDictionary<string, object> addAttributes) where TImg : Tag<TImg>
    {
        var l = Log.Fn<TImg>();
        if (addAttributes == null || !addAttributes.Any()) return l.Return(imgTag, "nothing to add");

        var dic = addAttributes
            .Where(pair => !Recipe.SpecialProperties.Contains(pair.Key, comparer: InvariantCultureIgnoreCase))
            .ToDictionary(p => p.Key, p => p.Value);
        if (!dic.Any()) return l.Return(imgTag, "only special props");

        l.A(ImgService.Debug, "will add properties from attributes");
        foreach (var a in dic)
            imgTag = imgTag.Attr(a.Key, a.Value);

        return l.Return(imgTag, "added");
    }

    private readonly GetOnce<Img> _imgTag = new();

    public IHtmlTag Tag => _tag.Get(GetTagWithToolbar);
    private readonly GetOnce<IHtmlTag> _tag = new();

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

    #region Toolbar

    /// <inheritdoc />
    // note: it's a method but ATM always returns the cached toolbar
    // still implemented as a method, so we could add future parameters if necessary
    // It's also marked as internal, because it's not yet final and may change
    public IToolbarBuilder Toolbar() => _toolbar.Get(() =>
    {
        var l = Log.Fn<IToolbarBuilder>();
        switch (Params.Toolbar)
        {
            case false: return l.ReturnNull("false");
            case IToolbarBuilder customToolbar: return l.Return(customToolbar, "already set");
        }

        // If we're creating an image for a string value, it won't have a field or parent.
        if (Params.Field?.Parent == null || Params.HasMetadataOrNull == null)
            return l.ReturnNull("no field or no metadata");

        // Determine if this is an "own" adam file, because only field-owned files should allow config
        var isInSameEntity = Security.PathIsInItemAdam(Params.Field.Parent.Guid, "", Src);

        // Construct the toolbar; in edge cases the toolbar service could be missing
        var imgTlb = ImgService.ToolbarOrNull?.Empty().Settings(
            hover: "right-middle",
            // Delay show of toolbar if it's a shared image, as it shouldn't be used much
            ui: isInSameEntity ? null : "delayShow=1000"
        );

        // Try to add the metadata button (or just null if not available)
        imgTlb = imgTlb?.Metadata(Params.HasMetadataOrNull,
            tweak: btns =>
            {
                // Note: Using experimental feature which doesn't exist on the ITweakButton interface

                // Add note only for the ImageDecorator Metadata, not for other buttons
                var modified = (btns as TweakButton)?.AddNamed(ImageDecorator.TypeNameId, btn =>
                {
                    // add label eg "Image Settings and Cropping" - i18n
                    btn = btn.Tooltip($"{ToolbarConstants.ToolbarLabelPrefix}MetadataImage");

                    // Try to add note
                    var note = (Params.Settings as ResizeSettings)?.ToHtmlInfo();
                    if (note.HasValue())
                        btn = btn.Note(note, format: "html", background: "#DFC2F2", delay: 1000);

                    // if image is from elsewhere, show warning
                    btn = isInSameEntity ? btn : btn.FormParameters(ShowWarningGlobalFile, true);
                    return btn;
                }) ?? btns; // fallback, in case conversion fails unexpectedly

                Params.Field?.Metadata
                    .OfType(CopyrightDecorator.NiceTypeName)
                    .FirstOrDefault()
                    .DoIfNotNull(cpEntity =>
                    {
                        var copyright = new CopyrightDecorator(cpEntity);
                        modified = (modified as TweakButton)?.AddNamed(CopyrightDecorator.TypeNameId, btn =>
                        {
                            btn = btn
                                .Tooltip("Copyright")
                                .Note(copyright.CopyrightMessage.NullIfNoValue() ?? copyright.Copyrights.FirstOrDefault()?.GetBestTitle() ?? "");
                            return btn;
                        }) ?? modified;
                    });


                return modified ?? btns;
            });

        return l.ReturnAsOk(imgTlb);
    });

    private readonly GetOnce<IToolbarBuilder> _toolbar = new();

    #endregion


    /// <inheritdoc />
    public string Description => _description.Get(() => Params.Field?.ImageDecoratorOrNull?.Description);
    private readonly GetOnce<string> _description = new();

    /// <inheritdoc />
    public string DescriptionExtended => _descriptionDet.Get(() => Params.Field?.ImageDecoratorOrNull?.DescriptionExtended);
    private readonly GetOnce<string> _descriptionDet = new();

    /// <inheritdoc />
    public string Alt => _alt.Get(() =>
        // If alt is specified, it takes precedence - even if it's an empty string, because there must have been a reason for this
        Params.ImgAlt 
        // If we take the image description, empty does NOT take precedence, it will be treated as not-set
        ?? Description.NullIfNoValue()
        // If all else fails, take the fallback specified in the call - IF it's allowed
        ?? (Params.Field?.ImageDecoratorOrNull?.SkipFallbackTitle ?? false ? null : Params.ImgAltFallback)
    );
    private readonly GetOnce<string> _alt = new();


    /// <inheritdoc />
    public string Class => _imgClass.Get(() => StyleOrClassGenerator(Params.ImgClass, Recipe.SpecialPropertyClass));
    private readonly GetOnce<string> _imgClass = new();

    private string StyleOrClassGenerator(string codePart, string key)
    {
        var l = (ImgService.Debug ? Log : null).Fn<string>();
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
    protected bool TryGetAttribute(IDictionary<string, object> attribs, string key, out string value)
    {
        value = null;
        if (attribs == null) return false;
        var found = attribs.TryGetValue(key, out var attrValue);
        value = attrValue?.ToString();
        return found && value.HasValue();
    }


    /// <inheritdoc />
    public bool ShowAll => ThisResize.ShowAll;

    /// <inheritdoc />
    public string SrcSet => _srcSet.Get(SrcSetGenerator);
    private readonly GetOnce<string> _srcSet = new();
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
    private readonly GetOnce<string> _width = new();

    /// <inheritdoc />
    public string Height => _height.Get(() => UseIfActive(ThisResize.Recipe?.SetHeight, ThisResize.Height));
    private readonly GetOnce<string> _height = new();

    /// <inheritdoc />
    public string Sizes => _sizes.Get(() => UseIfActive(ImgService.Features.IsEnabled(ImageServiceSetSizes.NameId), ThisResize.Recipe?.Sizes));
    private readonly GetOnce<string> _sizes = new();

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