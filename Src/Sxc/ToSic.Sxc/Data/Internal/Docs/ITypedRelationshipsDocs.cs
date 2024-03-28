namespace ToSic.Sxc.Data.Internal.Docs;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ITypedRelationshipsDocs
{
    /// <summary>
    /// A single item from a field.
    /// </summary>
    /// <param name="name">Name of the field</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="required">throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <returns>The ITypedItem. If the field doesn't exist or is empty, will return null.</returns>
    ITypedItem Child(string name, NoParamOrder noParamOrder = default, bool? required = default);

    /// <summary>
    /// A **typed** list of sub-items. Important for LINQ style querying or just
    /// working with various lists. Note that for getting child items of this item you
    /// can just use the properties, like content.Authors. <br/>
    /// But using Children("Authors", type: typeName) gives you the ability to restrict to a type.  <br/>
    /// Please check the tutorials on 2sxc.org/dnn-tutorials/ for more info. 
    /// </summary>
    /// <param name="field">Optional field filter - would only return items that point to the current item in a specific field name.</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="type">Optional type filter - would only return items of this type. </param>
    /// <param name="required">throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <returns>A list of all items pointing here (filtered), converted to DynamicEntity for convenience.</returns>
    /// <remarks>Note that the parameter-order is reversed to the Parents()</remarks>
    IEnumerable<ITypedItem> Children(string field = default, NoParamOrder noParamOrder = default, string type = default, bool? required = default);

    /// <summary>
    /// Get either the _current_ parent or the first parent which would be found on `.Parents(...)`.
    /// </summary>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="current">if set to `true`, will get the Item which created the current item (the parent) which called `.Child(...)` or `.Children(...)`</param>
    /// <param name="type">Optional type filter - would only return items of this type. </param>
    /// <param name="field">Optional field filter - would only return items that point to the current item in a specific field name.</param>
    /// <returns>_either_ the current parent _or_ the first parent returned by the same `.Parents(...)` call.</returns>
    ITypedItem Parent(NoParamOrder noParamOrder = default, bool? current = default, string type = default, string field = default);

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
    IEnumerable<ITypedItem> Parents(string type = default, NoParamOrder noParamOrder = default, string field = default);

}