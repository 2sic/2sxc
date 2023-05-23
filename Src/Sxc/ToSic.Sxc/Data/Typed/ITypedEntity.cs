using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Data
{
    [WorkInProgressApi("WIP")]
    public partial interface ITypedEntity: ICanBeEntity
    {
        int EntityId { get; }
        Guid EntityGuid { get; }
        dynamic Dyn { get; }
        TypedEntity Presentation { get; }
        //IDynamicMetadata Metadata { get; }
        IDynamicField Field(string name);
        object Get(string name);
        TValue Get<TValue>(string name, string noParamOrder = Eav.Parameters.Protector, TValue fallback = default);



        #region parents / children

        /// <summary>
        /// A **typed** list of entities which point to this item. Important for LINQ style querying or just
        /// working with various lists. Note that for getting child items of this item you
        /// can just use the properties, like content.Authors. <br/>
        /// Please check the tutorials on 2sxc.org/dnn-tutorials/ for more info. 
        /// </summary>
        /// <param name="type">Optional type filter - would only return items of this type. </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="field">Optional field filter - would only return items that point to the current item in a specific field name.</param>
        /// <returns>A list of all items pointing here (filtered), converted to DynamicEntity for convenience.</returns>
        /// <remarks>Note that the parameter-order is reversed to the Children()</remarks>
        IEnumerable<ITypedEntity> Parents(
            string type = null,
            string noParamOrder = Eav.Parameters.Protector,
            string field = null);

        /// <summary>
        /// A **typed** list of sub-items. Important for LINQ style querying or just
        /// working with various lists. Note that for getting child items of this item you
        /// can just use the properties, like content.Authors. <br/>
        /// But using Children("Authors", typeName) gives you the ability to restrict to a type.  <br/>
        /// Please check the tutorials on 2sxc.org/dnn-tutorials/ for more info. 
        /// </summary>
        /// <param name="type">Optional type filter - would only return items of this type. </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="field">Optional field filter - would only return items that point to the current item in a specific field name.</param>
        /// <returns>A list of all items pointing here (filtered), converted to DynamicEntity for convenience.</returns>
        /// <remarks>Note that the parameter-order is reversed to the Parents()</remarks>
        IEnumerable<ITypedEntity> Children(
            string field = null,
            string noParamOrder = Eav.Parameters.Protector,
            string type = null);

        /// <summary>
        /// A single item from a field.
        /// If the field doesn't exist or is empty, will return null.
        /// </summary>
        /// <param name="field">Name of the field</param>
        /// <returns></returns>
        ITypedEntity Child(string field);

        #endregion 


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
            string classes = default,
            object imageSettings = default,
            bool? toolbar = default,
            bool debug = default
        );
    }
}