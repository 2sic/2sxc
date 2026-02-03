using ToSic.Sxc.Data.Options;
using ToSic.Sxc.Data.Sys.Docs;

namespace ToSic.Sxc.Data;

partial interface ITypedItem
{
    #region parents / children

    /// <inheritdoc cref="ITypedRelationshipsDocs.Child"/>
    ITypedItem? Child(string name, NoParamOrder npo = default, bool? required = default, GetRelatedOptions? options = default);

    /// <inheritdoc cref="ITypedRelationshipsDocs.Children"/>
    IEnumerable<ITypedItem> Children(string? field = default, NoParamOrder npo = default, string? type = default, bool? required = default, GetRelatedOptions? options = default);

    /// <inheritdoc cref="ITypedRelationshipsDocs.Parent"/>
    ITypedItem? Parent(NoParamOrder npo = default, bool? current = default, string? type = default, string? field = default, GetRelatedOptions? options = default);

    /// <inheritdoc cref="ITypedRelationshipsDocs.Parents"/>
    IEnumerable<ITypedItem> Parents(NoParamOrder npo = default, string? type = default, string? field = default, GetRelatedOptions? options = default);

    #endregion

    #region New Child<T> / Children<T>

    /// <summary>
    /// Get a child and return with specified custom type.
    /// </summary>
    /// <param name="name">Name of the field</param>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="required">throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <returns></returns>
    /// <remarks>
    /// New v17.05
    /// </remarks>
    public T? Child<T>(string name, NoParamOrder npo = default, bool? required = default, GetRelatedOptions? options = default)
        where T : class, IModelOfData, new();

    /// <summary>
    /// A **strongly typed** list of sub-items. Important for LINQ style querying or just
    /// working with various lists. Note that for getting child items of this item you
    /// can just use the properties, like content.Authors. <br/>
    /// But using Children("Authors", type: typeName) gives you the ability to restrict to a type.  <br/>
    /// Please check the tutorials on 2sxc.org/dnn-tutorials/ for more info. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="field">Name of the field</param>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="type">Optional type filter - would only return items of this type. </param>
    /// <param name="required">throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <returns></returns>
    /// <remarks>
    /// New v17.05
    /// Never null, unless explicitly requested with `nullIfNull`, otherwise it would return an empty list.
    /// </remarks>
    public IEnumerable<T> Children<T>(string? field = default, NoParamOrder npo = default,
        string? type = default, bool? required = default, GetRelatedOptions? options = default)
        where T : class, IModelOfData, new();

    /// <summary>
    /// Get either the _current_ parent or the first parent which would be found on `.Parents(...)` as **strongly typed**.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="current">if set to `true`, will get the Item which created the current item (the parent) which called `.Child(...)` or `.Children(...)`</param>
    /// <param name="type">Optional type filter - would only return items of this type. </param>
    /// <param name="field">Optional field filter - would only return items that point to the current item in a specific field name.</param>
    /// <returns>_either_ the current parent _or_ the first parent returned by the same `.Parents(...)` call.</returns>
    /// <returns></returns>
    /// <remarks>
    /// New v17.06
    /// </remarks>
    public T? Parent<T>(NoParamOrder npo = default, bool? current = default, string? type = default,
        string? field = default, GetRelatedOptions? options = default)
        where T : class, IModelOfData, new();

    /// <summary>
    /// A **typed** list of entities which point to this item. Important for LINQ style querying or just
    /// working with various lists. Note that for getting child items of this item you
    /// can just use the properties, like content.Authors. <br/>
    /// Please check the tutorials on 2sxc.org/dnn-tutorials/ for more info. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type">Optional type filter - would only return items of this type. If not specified (null) will use the name of T.</param>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="field">Optional field filter - would only return items that point to the current item in a specific field name.</param>
    /// <returns>A list of all items pointing here (filtered), converted to DynamicEntity for convenience.</returns>
    /// <remarks>Note that the parameter-order is reversed to the Children()</remarks>
    /// <returns></returns>
    /// <remarks>
    /// New v17.06
    /// Never null. If not found, will return an empty list.
    /// </remarks>
    public IEnumerable<T> Parents<T>(NoParamOrder npo = default,
        string? type = default, string? field = default, GetRelatedOptions? options = default)
        where T : class, IModelOfData, new();

    #endregion

    /// <summary>
    /// True if this item version is published.
    /// This means that the item can exist as published, or published-with-draft, showing the published version.
    /// 
    /// _Note that by default, end-users only see the published version and don't see any draft version._
    /// </summary>
    /// <remarks>New in v17, see also <see cref="Publishing"/></remarks>
    bool IsPublished { get; }

    IPublishing Publishing { get; }
}