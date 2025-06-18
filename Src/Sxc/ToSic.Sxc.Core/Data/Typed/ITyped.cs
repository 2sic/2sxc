using System.Text.Json.Serialization;
using ToSic.Sxc.Data.Internal.Convert;

namespace ToSic.Sxc.Data;

/// <summary>
/// Objects which usually wrap other objects to provide strictly typed access to properties.
/// </summary>
/// <remarks>
/// The object will have typed Methods to read properties like `.String(propName)`.
/// 
/// It is usually the result of a `AsTyped(something)` or `AsItem(...)` command.
/// It is meant to help Razor etc. access Entity and/or dynamic objects in a typed way.
/// 
/// History: Introduced in 16.02.
/// </remarks>
[PublicApi]
[JsonConverter(typeof(DynamicJsonConverter))] // we'll have to keep an eye on it for scenarios where ITypedItem also inherits from ITypedRead, and could have some surprises. But since the DynamicEntity was never meant to become json, probably there is no code out there that tries to do this. 
public partial interface ITyped
{
    /// <summary>
    /// Get a property and return the value as a `bool`.
    /// If conversion fails, will return default `false` or what is specified in the `fallback`.
    /// </summary>
    /// <param name="name">property name</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">_optional_ fallback if conversion fails</param>
    /// <param name="required">throw error if the `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <returns>Value as `bool`</returns>
    bool Bool(string name, NoParamOrder noParamOrder = default, bool fallback = default, bool? required = default);


    /// <summary>
    /// Get a property and return the value as a `DateTime`.
    /// If conversion fails, will return default `0001-01-01` or what is specified in the `fallback`.
    /// </summary>
    /// <param name="name">property name</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">_optional_ fallback if conversion fails</param>
    /// <param name="required">throw error if the `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <returns>Value as `DateTime`</returns>
    DateTime DateTime(string name, NoParamOrder noParamOrder = default, DateTime fallback = default, bool? required = default);

    /// <summary>
    /// Get a property and return the value as a `string`.
    /// If conversion fails, will return default `null` or what is specified in the `fallback`.
    /// </summary>
    /// <param name="name">property name</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">_optional_ fallback if conversion fails</param>
    /// <param name="required">throw error if the `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <param name="scrubHtml">
    /// If `true`, will remove all HTML tags from the string.
    /// If `p` will remove all `p` tags, if `div,span` will remove these tags.
    /// This is the same as using `Kit.Scrub.All(...)` or `.Only(...). For more detailed scrubbing, use the `Kit.Scrub`
    /// </param>
    /// <returns>Value as `string`</returns>
    string String(string name, NoParamOrder noParamOrder = default, string? fallback = default, bool? required = default, object? scrubHtml = default);

    #region Numbers




    /// <summary>
    /// Get a property and return the value as a `int`.
    /// If conversion fails, will return default `0` or what is specified in the `fallback`.
    /// </summary>
    /// <param name="name">property name</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">_optional_ fallback if conversion fails</param>
    /// <param name="required">throw error if the `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <returns>Value as `int`</returns>
    int Int(string name, NoParamOrder noParamOrder = default, int fallback = default, bool? required = default);


    /// <summary>
    /// Get a property and return the value as a `long`.
    /// If conversion fails, will return default `0` or what is specified in the `fallback`.
    /// </summary>
    /// <param name="name">property name</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">_optional_ fallback if conversion fails</param>
    /// <param name="required">throw error if the `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <returns>Value as `long`</returns>
    long Long(string name, NoParamOrder noParamOrder = default, long fallback = default, bool? required = default);

    /// <summary>
    /// Get a property and return the value as a `float`.
    /// If conversion fails, will return default `0` or what is specified in the `fallback`.
    /// </summary>
    /// <param name="name">property name</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">_optional_ fallback if conversion fails</param>
    /// <param name="required">throw error if the `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <returns>Value as `float`</returns>
    float Float(string name, NoParamOrder noParamOrder = default, float fallback = default, bool? required = default);


    /// <summary>
    /// Get a property and return the value as a `decimal`.
    /// If conversion fails, will return default `0` or what is specified in the `fallback`.
    /// </summary>
    /// <param name="name">property name</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">_optional_ fallback if conversion fails</param>
    /// <param name="required">throw error if the `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <returns>Value as `decimal`</returns>
    decimal Decimal(string name, NoParamOrder noParamOrder = default, decimal fallback = default, bool? required = default);

    /// <summary>
    /// Get a property and return the value as a `double`.
    /// If conversion fails, will return default `0` or what is specified in the `fallback`.
    /// </summary>
    /// <param name="name">property name</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">_optional_ fallback if conversion fails</param>
    /// <param name="required">throw error if the `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <returns>Value as `double`</returns>
    double Double(string name, NoParamOrder noParamOrder = default, double fallback = default, bool? required = default);

    #endregion

    /// <summary>
    /// Get a url from a field.
    /// It will do sanitation / url-corrections for special characters etc.
    /// 
    /// On TypedItems it will also auto-convert values such as `file:72` or `page:14`.
    /// </summary>
    /// <param name="name">The field name.</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">_optional_ fallback if conversion fails</param>
    /// <param name="required">throw error if the `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <returns>A url converted if possible. If the field contains anything else such as `hello` then it will not be modified.</returns>
    string Url(string name, NoParamOrder noParamOrder = default, string? fallback = default, bool? required = default);

    #region Debugging

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    string ToString();

    #endregion
}