using System;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data.Typed;
using ToSic.Sxc.Images;
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
        ) => _Cdf.CompatibilityLevel < Constants.CompatibilityLevel12
                // Only do compatibility check if used on DynamicEntity
                ? throw new NotSupportedException($"{nameof(Html)}(...) not supported in older Razor templates. Use Razor14, RazorPro or newer.")
                : TypedItemHelpers.Html(_Cdf, this, name: name, noParamOrder: noParamOrder, container: container,
                    toolbar: toolbar, imageSettings: imageSettings, required: false, debug: debug);

        IHtmlTag ITypedItem.Html(
            string name,
            string noParamOrder,
            object container,
            bool? toolbar,
            object imageSettings,
            bool? required,
            bool debug
        ) => TypedItemHelpers.Html(_Cdf, this, name: name, noParamOrder: noParamOrder, container: container,
            toolbar: toolbar, imageSettings: imageSettings, required: required, debug: debug);

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
        ) => TypedItemHelpers.Picture(cdf: _Cdf, item: this, name: name, noParamOrder: noParamOrder, settings: settings,
            factor: factor, width: width, imgAlt: imgAlt,
            imgAltFallback: imgAltFallback, imgClass: imgClass, recipe: recipe);
    }
}
