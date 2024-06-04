using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Data;

public partial interface ITyped : IHasKeys, ICanGetByName
{
    /// <inheritdoc cref="IHasKeys.ContainsKey"/>
    new bool ContainsKey(string name);

    /// <inheritdoc cref="IHasKeys.IsEmpty"/>
    new bool IsEmpty(string name, NoParamOrder noParamOrder = default, string language = default);
    // ^^^ new is just so it's in the docs

    /// <inheritdoc cref="IHasKeys.IsNotEmpty"/>
    new bool IsNotEmpty(string name, NoParamOrder noParamOrder = default, string language = default);
    // ^^^ new is just so it's in the docs

    /// <inheritdoc cref="IHasKeys.Keys"/>
    new IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string> only = default);


    /// <summary>
    /// Get a property.
    /// </summary>
    /// <param name="name">the property name like `Image` - or path to sub-property like `Author.Name` (new v15)</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="required">throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <param name="language">
    /// Optional language like `de`, `de-ch` or `de,en` to determine which values to check.
    /// Will ignore languages not in the data model.
    /// On items that don't have ML data it will be ignored. new v17.10
    /// </param>
    /// <returns>The result if found or null; or error if the object is in strict mode</returns>
    /// <remarks>
    /// * parameter `languages` added in 17.10
    /// </remarks>
    object Get(string name,
        NoParamOrder noParamOrder = default,
        bool? required = default,
        string language = default
        );

    /// <summary>
    /// Get a value using the name - and cast it to the expected strong type.
    /// For example to get an int even though it's stored as decimal.
    /// 
    /// Since the parameter `fallback` determines the type `TValue` you can just write this like
    /// `something.Get("Title", fallback: "no title")
    /// </summary>
    /// <typeparam name="TValue">
    /// The expected type, like `string`, `int`, etc.
    /// Note that you don't need to specify it, if you specify the `fallback` property.
    /// </typeparam>
    /// <param name="name">the property name like `Image` - or path to sub-property like `Author.Name` (new v15)</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">the fallback value to provide if not found</param>
    /// <param name="required">throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <param name="language">
    /// Optional language like `de`, `de-ch` or `de,en` to determine which values to check.
    /// Will ignore languages not in the data model.
    /// On items that don't have ML data it will be ignored. new v17.10
    /// </param>
    /// <returns>The typed value, or the `default` like `null` or `0` if casting isn't possible.</returns>
    /// <remarks>
    /// * Added in v15
    /// * parameter `languages` added in 17.10
    /// </remarks>
    TValue Get<TValue>(string name,
        NoParamOrder noParamOrder = default,
        TValue fallback = default,
        bool? required = default,
        string language = default);
}