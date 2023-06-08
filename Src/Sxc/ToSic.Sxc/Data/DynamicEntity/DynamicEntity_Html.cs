using System;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc/>
        [PrivateApi("WIP not yet released")]
        public IHtmlTag Html(
            string name,
            string noParamOrder = Protector,
            object container = default,
            bool? toolbar = default,
            object imageSettings = default,
            bool debug = default
        )
        {
            if (_Services.CompatibilityLevel < Constants.CompatibilityLevel12)
                throw new NotSupportedException($"{nameof(Html)}(...) not supported in older Razor templates. Use Hybrid14 or newer.");
            
            Protect(noParamOrder, $"{nameof(container)}, {nameof(imageSettings)}, {nameof(toolbar)}, {nameof(debug)}...");

            ThrowIfKitNotAvailable();

            return _Services.Kit.Cms.Html(Field(name), container: container, classes: null, imageSettings: imageSettings, debug: debug, toolbar: toolbar);
        }

        private void ThrowIfKitNotAvailable()
        {
            if (_Services.Kit == null)
                throw new NotSupportedException(
                    $"Trying to use {nameof(Html)} in a scenario where the {nameof(_Services.Kit)} is not available.");
        }
    }
}
