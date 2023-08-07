using System;
using System.Runtime.CompilerServices;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc/>
        [PrivateApi("Should not be documented here, as it should only be used on ITyped")]
        public IHtmlTag Html(
            string name,
            string noParamOrder = Protector,
            object container = default,
            bool? toolbar = default,
            object imageSettings = default,
            bool debug = default
        )
        {
            // Only do compatibility check if used on DynamicEntity
            if (_Services.AsC.CompatibilityLevel < Constants.CompatibilityLevel12)
                throw new NotSupportedException($"{nameof(Html)}(...) not supported in older Razor templates. Use Hybrid14 or newer.");

            return (this as ITypedItem).Html(name: name, noParamOrder: noParamOrder, container: container, toolbar: toolbar, imageSettings: imageSettings, debug: debug);
        }

        IHtmlTag ITypedItem.Html(
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

            var kit = GetServiceKitOrThrow();
            var field = (this as ITypedItem).Field(name, required: required);
            return kit.Cms.Html(field, container: container, classes: null, imageSettings: imageSettings, debug: debug, toolbar: toolbar);
        }

        /// <inheritdoc/>
        IResponsivePicture ITypedItem.Picture(
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
            var kit = GetServiceKitOrThrow();
            var field = (this as ITypedItem).Field(name, required: true);
            return field.Url.IsEmptyOrWs()
                ? null 
                : kit.Image.Picture(field, settings: settings, factor: factor, width: width, imgAlt: imgAlt, imgAltFallback: imgAltFallback, imgClass: imgClass, recipe: recipe);
        }


        private ServiceKit14 GetServiceKitOrThrow([CallerMemberName] string cName = default)
        {
            if (_Services == null)
                throw new NotSupportedException(
                    $"Trying to use {cName}(...) in a scenario where the {nameof(_Services)} is not available.");

            var kit = _Services.AsC.GetServiceKit14();
            return kit ?? throw new NotSupportedException(
                $"Trying to use {cName}(...) in a scenario where the {nameof(ServiceKit14)} is not available.");
        }
    }
}
