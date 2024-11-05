using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services.Tweaks;

namespace ToSic.Sxc.Data.Internal.Typed;

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
        var kit = cdf.GetServiceKitOrThrow();
        var field = item.Field(name, required: required);
        return kit.Cms.Html(field, container: container, classes: null, imageSettings: imageSettings, debug: debug, toolbar: toolbar, tweak: tweak);
    }

    public static IResponsivePicture Picture(
        CodeDataFactory cdf,
        ITypedItem item,
        string name,
        NoParamOrder noParamOrder,
        Func<ITweakMedia, ITweakMedia> tweak,
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
        var kit = cdf.GetServiceKitOrThrow();
        var field = item.Field(name, required: true);
        return field.Url.IsEmptyOrWs()
            ? null
            : kit.Image.Picture(field, tweak: tweak, settings: settings, factor: factor, width: width,
                imgAlt: imgAlt, imgAltFallback: imgAltFallback, 
                imgClass: imgClass, imgAttributes: imgAttributes, pictureClass: pictureClass, pictureAttributes: pictureAttributes,
                toolbar: toolbar, recipe: recipe);
    }

    public static IResponsiveImage Img(
        CodeDataFactory cdf,
        ITypedItem item,
        string name,
        NoParamOrder noParamOrder,
        Func<ITweakMedia, ITweakMedia> tweak,
        object settings,
        object factor,
        object width,
        string imgAlt,
        string imgAltFallback,
        string imgClass,
        object imgAttributes,
        object toolbar,
        object recipe
    )
    {
        var kit = cdf.GetServiceKitOrThrow();
        var field = item.Field(name, required: true);
        return field.Url.IsEmptyOrWs()
            ? null
            : kit.Image.Img(field, tweak: tweak, settings: settings, noParamOrder: noParamOrder, factor: factor, width: width,
                imgAlt: imgAlt, imgAltFallback: imgAltFallback,
                imgClass: imgClass, imgAttributes: imgAttributes,
                toolbar: toolbar, recipe: recipe);
    }


    public static string MaybeScrub(string value, object scrubHtml, Func<IScrub> scrubSvc)
    {
        if (value == null) return null;
        return scrubHtml switch
        {
            string scrubStr => scrubStr.HasValue()
                ? scrubSvc().Only(value, scrubStr.CsvToArrayWithoutEmpty())
                : value,
            bool scrubBln => scrubBln
                ? scrubSvc().All(value)
                : value,
            _ => value
        };
    }

}