using System;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Data
{
    public partial class TypedItem
    {
        /// <inheritdoc />
        public IHtmlTag Html(
            string name,
            string noParamOrder = Eav.Parameters.Protector,
            object container = default,
            //string classes = default,
            bool? toolbar = default,
            object imageSettings = default,
            bool debug = default
        )
        {
            if (_Services.CompatibilityLevel < Constants.CompatibilityLevel12)
                throw new NotSupportedException($"{nameof(Html)}(...) not supported in older Razor templates. Use Hybrid14 or newer.");

            Eav.Parameters.Protect(noParamOrder, $"{nameof(container)}, {nameof(imageSettings)}, {nameof(toolbar)}, {nameof(debug)}...");

            return _Services.Kit.Cms.Show(Field(name), container: container, imageSettings: imageSettings, debug: debug, toolbar: toolbar);
        }

    }
}
