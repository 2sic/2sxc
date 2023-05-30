using System;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// A typed object to access data.
    /// Previously Razor code always used `dynamic` <see cref="IDynamicEntity"/> objects.
    /// This had some disadvantages when working with LINQ or advanced toolbars.
    /// </summary>
    /// <remarks>
    /// Introduced in 2sxc 16.01
    /// </remarks>
    [PublicApi]
    public partial interface ITypedItem: ICanBeEntity
    {
        int EntityId { get; }
        Guid EntityGuid { get; }

        /// <summary>
        /// A dynamic accessor for properties, to quickly get values when you don't care about type safety.
        /// This actually is a classic <see cref="IDynamicEntity"/>.
        ///
        /// Example: `Dyn.FirstName` will just work - and return the first name or `null` if not found.
        /// 
        /// </summary>
        dynamic Dyn { get; }

        /// <summary>
        /// The presentation item or `null` if it doesn't exist.
        /// </summary>
        ITypedItem Presentation { get; }

        //IDynamicMetadata Metadata { get; }


        IDynamicField Field(string name);
        object Get(string name);
        TValue Get<TValue>(string name, string noParamOrder = Eav.Parameters.Protector, TValue fallback = default);






        /// <summary>
        /// Show a field in the expected / best possible way.
        /// As of now it's meant for WYSIWYG fields with Very-Rich Text.
        /// Docs will follow - TODO
        /// </summary>
        /// <param name="name">the field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
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
            //string classes = default,
            bool? toolbar = default,
            object imageSettings = default,
            bool debug = default
        );
    }
}