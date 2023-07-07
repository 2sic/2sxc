using System;
using System.Runtime.CompilerServices;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Sxc.Services;
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
            if (_Services.AsC.CompatibilityLevel < Constants.CompatibilityLevel12)
                throw new NotSupportedException($"{nameof(Html)}(...) not supported in older Razor templates. Use Hybrid14 or newer.");
            
            Protect(noParamOrder, $"{nameof(container)}, {nameof(imageSettings)}, {nameof(toolbar)}, {nameof(debug)}...");

            var kit = GetServiceKitOrThrow();

            return kit.Cms.Html(Field(name), container: container, classes: null, imageSettings: imageSettings, debug: debug, toolbar: toolbar);
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
