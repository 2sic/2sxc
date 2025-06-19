using ToSic.Razor.Blade;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services.Tweaks;

namespace ToSic.Sxc.Data.Internal.Typed;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class TypedItemHelpers
{
    public static IHtmlTag? Html(
        ICodeDataFactory cdf,
        ITypedItem item,
        string name,
        NoParamOrder noParamOrder,
        object? container,
        bool? toolbar,
        object? imageSettings,
        bool? required,
        bool debug,
        Func<ITweakInput<string>, ITweakInput<string>>? tweak = default
    )
    {
        var field = item.Field(name, required: required);
        if (field == null)
            return null;
        return cdf.Html(field, container: container, classes: null, imageSettings: imageSettings, debug: debug, toolbar: toolbar, tweak: tweak);
    }

    public static IResponsivePicture? Picture(
        ICodeDataFactory cdf,
        ITypedItem item,
        string name,
        NoParamOrder noParamOrder,
        Func<ITweakMedia, ITweakMedia>? tweak,
        object? settings,
        object? factor,
        object? width,
        string? imgAlt,
        string? imgAltFallback,
        string? imgClass,
        object? imgAttributes,
        string? pictureClass,
        object? pictureAttributes,
        object? toolbar,
        object? recipe
    )
    {
        var field = item.Field(name, required: true);
        if (field == null || field.Url.IsEmptyOrWs())
            return null;
        return cdf.Picture(field, tweak: tweak, settings: settings, factor: factor, width: width,
                imgAlt: imgAlt, imgAltFallback: imgAltFallback, 
                imgClass: imgClass, imgAttributes: imgAttributes, pictureClass: pictureClass, pictureAttributes: pictureAttributes,
                toolbar: toolbar, recipe: recipe);
    }

    public static IResponsiveImage? Img(
        ICodeDataFactory cdf,
        ITypedItem item,
        string name,
        NoParamOrder noParamOrder,
        Func<ITweakMedia, ITweakMedia>? tweak,
        object? settings,
        object? factor,
        object? width,
        string? imgAlt,
        string? imgAltFallback,
        string? imgClass,
        object? imgAttributes,
        object? toolbar,
        object? recipe
    )
    {
        var field = item.Field(name, required: true);
        if (field == null || field.Url.IsEmptyOrWs())
            return null;
        return cdf.Img(field, tweak: tweak, settings: settings, noParamOrder: noParamOrder, factor: factor, width: width,
                imgAlt: imgAlt, imgAltFallback: imgAltFallback,
                imgClass: imgClass, imgAttributes: imgAttributes,
                toolbar: toolbar, recipe: recipe);
    }


    public static string? MaybeScrub(string? value, object? scrubHtml, Func<IScrub> scrubSvc)
    {
        if (value == null)
            return null;
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