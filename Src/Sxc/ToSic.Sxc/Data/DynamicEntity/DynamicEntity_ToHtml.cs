using System;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <summary>
        /// Show a field in the expected / best possible way.
        /// As of now it's meant for WYSIWYG fields with Very-Rich Text.
        /// Docs will follow - TODO
        /// </summary>
        /// <param name="name">the field name</param>
        /// <param name="noParamOrder"></param>
        /// <param name="container">
        /// A wrapper tag for the result.
        /// It's either a RazorBlade tag such as `Kit.HtmlTag.Div()`, a string such as `span` or an empty string `` to indicate no container.
        /// If not set it will default to to a div-tag.
        /// </param>
        /// <param name="classes">Classes to add to the container.</param>
        /// <param name="imageSettings">Settings for resizing. Default is `Wysiwyg` but it can also be `Content` or a settings object.</param>
        /// <param name="debug">Activate debug visualization to better see alignments and such.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        [PrivateApi("WIP not yet released")]
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
