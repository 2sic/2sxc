using ToSic.Razor.Blade;

namespace ToSic.Sxc.Data;

public partial interface IDynamicEntity
{
    /// <summary>
    /// Many templates show demo data.
    /// If the template code must know if it's the demo item or
    /// real data, use `.IsDemoItem`.
    /// </summary>
    /// <returns>
    /// True if this is the item configured in the view-settings, false if not.
    /// </returns>
    /// <remarks>New in 10.07 on IDynamicEntity, new in 16.02 on ITypedEntity</remarks>
    bool IsDemoItem { get; }

    /// <summary>
    /// Show a field in the expected / best possible way.
    /// As of now it's meant for WYSIWYG fields with Very-Rich Text.
    /// See [](xref:NetCode.DynamicData.DynamicEntityHtml)
    /// </summary>
    /// <param name="name">the field name</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="container">
    /// A wrapper tag for the result.
    /// It's either a RazorBlade tag such as `Kit.HtmlTag.Div()`, a string such as `span` or an empty string `` to indicate no container.
    /// If not set it will default to to a div-tag.
    /// See [docs](xref:NetCode.DynamicData.DynamicEntityHtml)
    /// </param>
    /// <param name="toolbar">Override default toolbar behavior on this field. See [docs](xref:NetCode.DynamicData.DynamicEntityHtml)</param>
    /// <param name="imageSettings">Settings for resizing. Default is `Wysiwyg` but it can also be `Content` or a settings object.</param>
    /// <param name="debug">Activate debug visualization to better see alignments and such.</param>
    /// <returns></returns>
    /// <remarks>
    /// * Added in 2sxc 16.01
    /// * Only works on Razor files inheriting from Hybrid14 or newer
    /// </remarks>
    IHtmlTag Html(
        string name,
        NoParamOrder noParamOrder = default,
        object container = default,
        bool? toolbar = default,
        object imageSettings = default,
        bool debug = default
    );
}