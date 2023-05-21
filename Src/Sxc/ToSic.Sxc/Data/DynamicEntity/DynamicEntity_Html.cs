using System;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc/>
        [PrivateApi("WIP not yet released")]
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

            return _Services.Kit.Cms.Show(Field(name), container: container, classes: null, imageSettings: imageSettings, debug: debug, toolbar: toolbar);
        }
        [PrivateApi("old command, remove after upgrades")]
        public IHtmlTag ToHtml(
            string name,
            string noParamOrder = Eav.Parameters.Protector,
            object container = default,
            string classes = default,
            object imageSettings = default,
            bool debug = default
        )
        {
            if (_Services.CompatibilityLevel < Constants.CompatibilityLevel12)
                throw new NotSupportedException($"{nameof(ToHtml)}(...) not supported in older Razor templates. Use Hybrid14 or newer.");

            return _Services.Kit.Cms.Show(Field(name), container: container, classes: classes, imageSettings: imageSettings, debug: debug);
        }
    }
}
