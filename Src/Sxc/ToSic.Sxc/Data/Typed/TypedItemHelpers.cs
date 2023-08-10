using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;
using ToSic.Sxc.Images;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data.Typed
{
    internal class TypedItemHelpers
    {
        public static IHtmlTag Html(
            CodeDataFactory cdf,
            ITypedItem item,
            string name,
            string noParamOrder,
            object container,
            bool? toolbar,
            object imageSettings,
            bool? required,
            bool debug
        )
        {
            Protect(noParamOrder, $"{nameof(container)}, {nameof(imageSettings)}, {nameof(toolbar)}, {nameof(required)}, {nameof(debug)}...");
            var kit = cdf.GetServiceKitOrThrow();
            var field = item.Field(name, required: required);
            return kit.Cms.Html(field, container: container, classes: null, imageSettings: imageSettings, debug: debug, toolbar: toolbar);
        }

        public static IResponsivePicture Picture(
            CodeDataFactory cdf,
            ITypedItem item,
            string name,
            string noParamOrder,
            object settings,
            object factor,
            object width,
            string imgAlt,
            string imgAltFallback,
            string imgClass,
            object recipe
        )
        {
            Protect(noParamOrder, $"{nameof(settings)}, {nameof(factor)}, {nameof(width)}, {nameof(imgAlt)}...");
            var kit = cdf.GetServiceKitOrThrow();
            var field = item.Field(name, required: true);
            return field.Url.IsEmptyOrWs()
                ? null
                : kit.Image.Picture(field, settings: settings, factor: factor, width: width, imgAlt: imgAlt, imgAltFallback: imgAltFallback, imgClass: imgClass, recipe: recipe);
        }

    }
}
