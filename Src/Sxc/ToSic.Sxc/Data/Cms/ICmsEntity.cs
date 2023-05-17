using System;
using ToSic.Eav.Data;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Data
{
    public interface ICmsEntity: ICanBeEntity
    {
        new CmsEntity Presentation { get; }
        int Id { get; }
        Guid Guid { get; }
        dynamic Dyn { get; }
        IDynamicMetadata Metadata { get; }

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
        IHtmlTag Html(
            string name,
            string noParamOrder = Eav.Parameters.Protector,
            object container = default,
            string classes = default,
            object imageSettings = default,
            bool? toolbar = default,
            bool debug = default
        );

        
        object Get(string name);
        TValue Get<TValue>(string name, string noParamOrder = Eav.Parameters.Protector, TValue fallback = default);
        string String(string name, string fallback = default);
        int Int(string name, int fallback = default);
        bool Bool(string name, bool fallback = default);
        long Long(string name, long fallback = default);
        decimal Decimal(string name, decimal fallback = default);
        double Double(string name, double fallback = default);
        IDynamicField Field(string name);
    }
}