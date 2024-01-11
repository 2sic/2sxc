namespace ToSic.Sxc.Data.Internal.Docs;

/// <summary>
/// This is minor cross-concerns aspect of Dynamic-Entity-like objects
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal abstract class DynamicEntityDocs
{
    /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityBase and IDynamicStack */
    /// <summary>
    /// Get a value of the entity. Usually you will prefer the quick access like
    /// @content.FirstName - which will give you the same things as content.Get("FirstName").
    /// There are two cases to use this:
    /// - when you dynamically assemble the field name in your code, like when using App.Resources or similar use cases.
    /// - to access a field which has a conflicting name with this object, like Get("Parents")
    /// </summary>
    /// <param name="name"></param>
    /// <returns>An object which can be either a string, number, boolean or List&lt;IDynamicEntity&gt;, depending on the field type. Will return null if the field was not found. </returns>
    public abstract dynamic Get(string name);


    /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityBase and IDynamicStack */
    /// <summary>
    /// Get a property using the string name. Only needed in special situations, as most cases can use the object.name directly
    /// </summary>
    /// <param name="name">the property name like `Image` - or path like `Author.Name` (new v15)</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="language">Optional language code - like "de-ch" to prioritize that language</param>
    /// <param name="convertLinks">Optionally turn off if links like file:72 are looked up to a real link. Default is true.</param>
    /// <param name="debug">Set true to see more details in [Insights](xref:NetCode.Debug.Insights.Index) how the value was retrieved.</param>
    /// <returns>a dynamically typed result, can be string, bool, etc.</returns>
    public abstract dynamic Get(string name,
        // ReSharper disable once MethodOverloadWithOptionalParameter
        NoParamOrder noParamOrder = default,
        string language = null,
        bool convertLinks = true,
        bool? debug = null
    );

    /// <summary>
    /// Get a value using the name - and cast it to the expected strong type.
    /// For example to get an int even though it's stored as decimal.
    /// </summary>
    /// <typeparam name="TValue">The expected type, like `string`, `int`, etc.</typeparam>
    /// <param name="name">the property name like `Image` - or path like `Author.Name` (new v15)</param>
    /// <returns>The typed value, or the `default` like `null` or `0` if casting isn't possible.</returns>
    /// <remarks>Added in v15</remarks>
    public abstract TValue Get<TValue>(string name);

    /// <summary>
    /// Get a value using the name - and cast it to the expected strong type.
    /// For example to get an int even though it's stored as decimal.
    /// 
    /// Since the parameter `fallback` determines the type `TValue` you can just write this like
    /// `Content.Get("Title", fallback: "no title")
    /// </summary>
    /// <typeparam name="TValue">
    /// The expected type, like `string`, `int`, etc.
    /// Note that you don't need to specify it, if you specify the `fallback` property.
    /// </typeparam>
    /// <param name="name">the property name like `Image` - or path like `Author.Name` (new v15)</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">the fallback value to provide if not found</param>
    /// <returns>The typed value, or the `default` like `null` or `0` if casting isn't possible.</returns>
    /// <remarks>Added in v15</remarks>
    public abstract TValue Get<TValue>(string name,
        // ReSharper disable once MethodOverloadWithOptionalParameter
        NoParamOrder noParamOrder = default,
        TValue fallback = default);

}