using ToSic.Sxc.Code.Internal.Documentation;
using ToSic.Sxc.Data;

// 2024-01-22 2dm
// Remove all convert methods which are just missing the optional parameters, to make the API smaller.
// Assume it has no side effects, must watch.
// Remove this note 2024-Q3 (ca. July)

namespace ToSic.Sxc.Services;

/// <summary>
/// Conversion helper for things which are very common in web-code like Razor and WebAPIs.
/// It's mainly a safe conversion from anything to a target-type.
/// 
/// Some special things it does:
/// * Strings like "4.2" reliably get converted to int 4 which would otherwise return 0
/// * Numbers like 42 reliably converts to bool true which would otherwise return false
/// * Numbers like 42.5 reliably convert to strings "42.5" instead of "42,5" in certain cultures
/// </summary>
/// <remarks>
/// * New in v16.03
/// * Difference to <see cref="IConvertService"/> is that the param `fallback` must always be named
/// </remarks>
[PublicApi]
[Docs(AutoLink = true, Messages = ["Helper to convert any object into another - like a string to int."])]
public interface IConvertService16
{
    // Important internal information
    // Most of our APIs have a pleaseNameParams parameter very early on
    // But these don't enforce that for the second fallback parameter.
    // The reason is that they are very common, and if people would try
    // ToInt(value, 27) they would always get a
    // "second parameter is not a string" - which is hard for people to figure out why this happens

    ///// <summary>
    ///// Convert any object safely to the desired type T.
    ///// If conversion fails, it will return `default(T)`, which is 0 for most numbers, `false` for boolean or `null` for strings or objects.
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="value"></param>
    ///// <returns></returns>
    //T To<T>(object value);

    /// <summary>
    /// Convert any object safely to the desired type.
    /// If conversion fails, it will return the `fallback` parameter as given, or `default(T)`.
    /// Since the fallback is typed, you can usually call this method without specifying T explicitly, so this should work:
    /// 
    /// ```
    /// var c1 = Convert.To("5", fallback: 100); // will return 5
    /// var c2 = Convert.To("", fallback: 100);  // will return 100
    /// var c1 = Convert.To(""); // will return 0
    /// ```
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">value to convert</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The value used if conversion fails. If not specified, will use `default(T)`</param>
    /// <returns></returns>
    T To<T>(object value, NoParamOrder noParamOrder = default, T fallback = default);

    ///// <summary>
    ///// Convert any object safely to bool.
    ///// This does the same as <see cref="To{T}(object, NoParamOrder, T)"/> but this is easier to type in Razor.
    /////
    ///// _Note that it's called ToBool, not ToBoolean, because the core type is also called bool, not boolean. This is different from `System.Convert.ToBoolean(...)`_
    ///// </summary>
    //bool ToBool(object value);

    /// <summary>
    /// Convert any object safely to bool, or if that fails, return the fallback value.
    /// 
    /// _Note that it's called ToBool, not ToBoolean, because the core type is also called bool, not boolean. This is different from `System.Convert.ToBoolean(...)`_
    /// </summary>
    /// <param name="value">value to convert</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The value used if conversion fails. Defaults to `false`.</param>
    /// <returns></returns>
    bool ToBool(object value, NoParamOrder noParamOrder = default, bool fallback = default);

    ///// <summary>
    ///// Convert any object safely to decimal.
    ///// This does the same as <see cref="To{T}(object, NoParamOrder, T)"/> but this is easier to type in Razor.
    ///// </summary>
    //decimal ToDecimal(object value);

    /// <summary>
    /// Convert any object safely to decimal, or if that fails, return the fallback value.
    /// This does the same as <see cref="To{T}(object, NoParamOrder, T)"/> but this is easier to type in Razor.
    /// </summary>
    /// <param name="value">value to convert</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The value used if conversion fails. Defaults to `0`.</param>
    decimal ToDecimal(object value, NoParamOrder noParamOrder = default, decimal fallback = default);

    ///// <summary>
    ///// Convert any object safely to double.
    ///// This does the same as <see cref="To{T}(object, NoParamOrder, T)"/> but this is easier to type in Razor.
    ///// </summary>
    //double ToDouble(object value);

    /// <summary>
    /// Convert any object safely to double, or if that fails, return the fallback value.
    /// This does the same as <see cref="To{T}(object, NoParamOrder, T)"/> but this is easier to type in Razor.
    /// </summary>
    /// <param name="value">value to convert</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The value used if conversion fails. Defaults to `0`.</param>
    double ToDouble(object value, NoParamOrder noParamOrder = default, double fallback = default);

    ///// <summary>
    ///// Convert any object safely to float.
    ///// This does the same as <see cref="To{T}(object, NoParamOrder, T)"/> but this is easier to type in Razor.
    /////
    ///// _Note that it's called ToFloat, not ToSingle, because the core type is also called float, not single. This is different from `System.Convert.ToSingle(...)`_
    ///// </summary>
    //float ToFloat(object value);

    /// <summary>
    /// Convert any object safely to float, or if that fails, return the fallback value.
    /// This does the same as <see cref="To{T}(object, NoParamOrder, T)"/> but this is easier to type in Razor.
    ///
    /// _Note that it's called ToFloat, not ToSingle, because the core type is also called float, not single. This is different from `System.Convert.ToSingle(...)`_
    /// </summary>
    /// <param name="value">value to convert</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The value used if conversion fails. Defaults to `0`.</param>
    float ToFloat(object value, NoParamOrder noParamOrder = default, float fallback = default);

    ///// <summary>
    ///// Convert any object safely to standard int.
    ///// This does the same as <see cref="To{T}(object, NoParamOrder, T)"/> but this is easier to type in Razor.
    ///// </summary>
    //int ToInt(object value);

    /// <summary>
    /// Convert any object safely to standard int, or if that fails, return the fallback value.
    /// This does the same as <see cref="To{T}(object, NoParamOrder, T)"/> but this is easier to type in Razor.
    /// </summary>
    /// <param name="value">value to convert</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The value used if conversion fails. Defaults to `0`.</param>
    int ToInt(object value, NoParamOrder noParamOrder = default, int fallback = default);

    ///// <summary>
    ///// Convert any object safely to a Guid
    ///// This does the same as <see cref="To{T}(object, NoParamOrder, T)"/> but this is easier to type in Razor.
    ///// </summary>
    //Guid ToGuid(object value);

    /// <summary>
    /// Convert any object safely to standard guid, or if that fails, return the fallback value.
    /// This does the same as <see cref="To{T}(object, NoParamOrder, T)"/> but this is easier to type in Razor.
    /// </summary>
    /// <param name="value">value to convert</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The value used if conversion fails. Defaults to `Guid.Empty`.</param>
    Guid ToGuid(object value, NoParamOrder noParamOrder = default, Guid fallback = default);



    ///// <summary>
    ///// Convert any object safely to string.
    ///// This does the same as <see cref="To{T}(object, NoParamOrder, T)"/> but this is easier to type in Razor.
    ///// </summary>
    //string ToString(object value);

    /// <summary>
    /// Convert any object safely to string - or if that fails, return the fallback value.
    /// 
    /// This does **NOT** do the same as <see cref="To{T}(object, NoParamOrder, T)"/>.
    /// In the standard implementation would only give you the fallback, if conversion failed.
    /// But this ToString will also give you the fallback, if the result is null. 
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">Fallback in case conversion fails or result is null. Defaults to `null`.</param>
    /// <param name="fallbackOnNull">Determine that nulls should also fallback, default is `true`</param>
    string ToString(object value,
        NoParamOrder noParamOrder = default,
        string fallback = default,
        bool fallbackOnNull = true);

    ///// <summary>
    ///// Convert any object safely to string to put into source code like HTML-attributes, inline-JavaScript or similar.
    ///// This is usually used to ensure numbers, booleans and dates are in a format which works.
    ///// Especially useful when giving data to a JavaScript, Json-Fragment or an Html Attribute.
    /////
    ///// * booleans will be `true` or `false` (not `True` or `False`)
    ///// * numbers will have a . notation and never a comma (like in de-DE cultures)
    ///// * dates will convert to ISO format without time zone
    ///// </summary>
    //string ForCode(object value);

    /// <summary>
    /// Convert any object safely to string to put into source code like HTML-attributes, inline-JavaScript or similar.
    /// This is usually used to ensure numbers, booleans and dates are in a format which works.
    /// Especially useful when giving data to a JavaScript, Json-Fragment or an Html Attribute.
    ///
    /// * booleans will be `true` or `false` (not `True` or `False`)
    /// * numbers will have a . notation and never a comma (like in de-DE cultures)
    /// * dates will convert to ISO format without time zone
    /// 
    /// Optionally also allows a `fallback` to use instead of the defaults above.
    /// </summary>
    /// <returns></returns>
    /// <param name="value">value to convert</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The value used if conversion fails. Defaults to `null`.</param>
    string ForCode(object value, NoParamOrder noParamOrder = default, string fallback = default);

    /// <summary>
    /// Sub-Service to convert JSON
    /// </summary>
    IJsonService Json { get; }

    #region New v17 As conversions

    [PrivateApi("WIP, don't publish yet")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    T As<T>(ICanBeEntity source, NoParamOrder protector = default, bool nullIfNull = false)
        where T : class, ITypedItemWrapper16, ITypedItem, new();

    [PrivateApi("WIP, don't publish yet")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    IEnumerable<T> AsList<T>(IEnumerable<ICanBeEntity> source, NoParamOrder protector = default, bool nullIfNull = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new();

    #endregion
}