using System;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Data
{
    public partial class TypedEntity
    {
        [PrivateApi("WIP not yet released")]
        public IHtmlTag Html(
            string name,
            string noParamOrder = Eav.Parameters.Protector,
            object container = default,
            string classes = default,
            object imageSettings = default,
            bool? toolbar = default,
            bool debug = default
        )
        {
            if (_Services.CompatibilityLevel < Constants.CompatibilityLevel12)
                throw new NotSupportedException($"{nameof(Html)}(...) not supported in older Razor templates. Use Hybrid14 or newer.");

            Eav.Parameters.Protect(noParamOrder, $"{nameof(container)}, {nameof(classes)}, {nameof(imageSettings)}, {nameof(toolbar)}, {nameof(debug)}...");

            return _Services.Kit.Cms.Show(Field(name), container: container, classes: classes, imageSettings: imageSettings, debug: debug, toolbar: toolbar);
        }

    }
}
