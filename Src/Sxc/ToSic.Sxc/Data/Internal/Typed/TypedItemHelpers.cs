using System.Linq;
using System;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Coding;
using ToSic.Razor.Blade;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services.Tweaks;

namespace ToSic.Sxc.Data.Typed;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class TypedItemHelpers
{
    public static IHtmlTag Html(
        CodeDataFactory cdf,
        ITypedItem item,
        string name,
        NoParamOrder noParamOrder,
        object container,
        bool? toolbar,
        object imageSettings,
        bool? required,
        bool debug,
        Func<ITweakInput<string>, ITweakInput<string>> tweak = default
    )
    {
        // Protect(noParamOrder, $"{nameof(container)}, {nameof(imageSettings)}, {nameof(toolbar)}, {nameof(required)}, {nameof(debug)}...");
        var kit = cdf.GetServiceKitOrThrow();
        var field = item.Field(name, required: required);
        return kit.Cms.Html(field, container: container, classes: null, imageSettings: imageSettings, debug: debug, toolbar: toolbar, tweak: tweak);
    }

    public static IResponsivePicture Picture(
        CodeDataFactory cdf,
        ITypedItem item,
        string name,
        NoParamOrder noParamOrder,
        object settings,
        object factor,
        object width,
        string imgAlt,
        string imgAltFallback,
        string imgClass,
        object imgAttributes,
        string pictureClass,
        object pictureAttributes,
        object toolbar,
        object recipe
    )
    {
        //Protect(noParamOrder, $"{nameof(settings)}, {nameof(factor)}, {nameof(width)}, {nameof(imgAlt)}...");
        var kit = cdf.GetServiceKitOrThrow();
        var field = item.Field(name, required: true);
        return field.Url.IsEmptyOrWs()
            ? null
            : kit.Image.Picture(field, settings: settings, factor: factor, width: width,
                imgAlt: imgAlt, imgAltFallback: imgAltFallback, 
                imgClass: imgClass, imgAttributes: imgAttributes, pictureClass: pictureClass, pictureAttributes: pictureAttributes,
                toolbar: toolbar, recipe: recipe);
    }

    public static string MaybeScrub(string value, object scrubHtml, Func<IScrub> scrubSvc)
    {
        if (value == null) return null;
        switch (scrubHtml)
        {
            case string scrubStr:
                return scrubStr.HasValue()
                    ? scrubSvc().Only(value, scrubStr.Split(',').Select(s => s.Trim()).ToArray())
                    : value;
            case bool scrubBln:
                return scrubBln ? scrubSvc().All(value) : value;
            default:
                return value;
        }
    }

}