using ToSic.Sxc.Code.Internal.Documentation;

// ReSharper disable MethodOverloadWithOptionalParameter

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
/// New in v12.05
/// </remarks>
[PublicApi]
[Docs(AutoLink = true, Messages = ["Helper to convert any object into another - like a string to int."])]
public interface IConvertService
{
    // Important internal information
    // Most of our APIs have a pleaseNameParams parameter very early on
    // But these don't enforce that for the second fallback parameter.
    // The reason is that they are very common, and if people would try
    // ToInt(value, 27) they would always get a
    // "second parameter is not a string" - which is hard for people to figure out why this happens


    ///// <summary>
    ///// If set to true (default) will optimize converting numbers.
    ///// For example, a string like "4.2" will properly convert to an int of 4.
    ///// If set to false, this optimization doesn't happen and a string "4.2" would result in a 0 int
    ///// </summary>
    //[PrivateApi("Set to private in 16.03 as it was never communicated and it's read-only anyhow, so it can't be in use")]
    //bool OptimizeNumbers { get; }

    ///// <summary>
    ///// If set to true, will treat a number like 2 or -1 and strings like "2" as true.
    ///// If set to false, only 1 will be true, other numbers will be false.
    ///// </summary>
    //[PrivateApi("Set to private in 16.03 as it was never communicated and it's read-only anyhow, so it can't be in use")]
    //bool OptimizeBoolean { get; }

    /// <summary>
    /// Convert any object safely to the desired type T.
    /// If conversion fails, it will return `default(T)`, which is 0 for most numbers, `false` for boolean or `null` for strings or objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    T To<T>(object value);

    /// <summary>
    /// Convert any object safely to the desired type T.
    /// If conversion fails, it will return the `fallback` parameter as given.
    /// Since the fallback is typed, you can usually call this method without specifying T explicitly, so this should work:
    /// 
    /// ```
    /// var c1 = Convert.To("5", 100); // will return 5
    /// var c2 = Convert.To("", 100);  // will return 100
    /// ```
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The value used if conversion fails.</param>
    /// <returns></returns>
    T To<T>(object value,
        NoParamOrder noParamOrder = default,
        T fallback = default);

    /// <summary>
    /// Convert any object safely to bool.
    /// This does the same as <see cref="To{T}(object)"/> but this is easier to type in Razor.
    ///
    /// _Note that it's called ToBool, not ToBoolean, because the core type is also called bool, not boolean. This is different from `System.Convert.ToBoolean(...)`_
    /// </summary>
    bool ToBool(object value);

    /// <summary>
    /// Convert any object safely to bool, or if that fails, return the fallback value.
    /// 
    /// _Note that it's called ToBool, not ToBoolean, because the core type is also called bool, not boolean. This is different from `System.Convert.ToBoolean(...)`_
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fallback"></param>
    /// <returns></returns>
    bool ToBool(object value, bool fallback = default);

    /// <summary>
    /// Convert any object safely to decimal.
    /// This does the same as <see cref="To{T}(object)"/> but this is easier to type in Razor.
    /// </summary>
    decimal ToDecimal(object value);

    /// <summary>
    /// Convert any object safely to decimal, or if that fails, return the fallback value.
    /// This does the same as <see cref="To{T}(object)"/> but this is easier to type in Razor.
    /// </summary>
    decimal ToDecimal(object value, decimal fallback = default);

    /// <summary>
    /// Convert any object safely to double.
    /// This does the same as <see cref="To{T}(object)"/> but this is easier to type in Razor.
    /// </summary>
    double ToDouble(object value);

    /// <summary>
    /// Convert any object safely to double, or if that fails, return the fallback value.
    /// This does the same as <see cref="To{T}(object)"/> but this is easier to type in Razor.
    /// </summary>
    double ToDouble(object value, double fallback = default);

    /// <summary>
    /// Convert any object safely to float.
    /// This does the same as <see cref="To{T}(object)"/> but this is easier to type in Razor.
    ///
    /// _Note that it's called ToFloat, not ToSingle, because the core type is also called float, not single. This is different from `System.Convert.ToSingle(...)`_
    /// </summary>
    float ToFloat(object value);

    /// <summary>
    /// Convert any object safely to float, or if that fails, return the fallback value.
    /// This does the same as <see cref="To{T}(object)"/> but this is easier to type in Razor.
    ///
    /// _Note that it's called ToFloat, not ToSingle, because the core type is also called float, not single. This is different from `System.Convert.ToSingle(...)`_
    /// </summary>
    float ToFloat(object value, float fallback = default);

    /// <summary>
    /// Convert any object safely to standard int.
    /// This does the same as <see cref="To{T}(object)"/> but this is easier to type in Razor.
    /// </summary>
    int ToInt(object value);

    /// <summary>
    /// Convert any object safely to standard int, or if that fails, return the fallback value.
    /// This does the same as <see cref="To{T}(object)"/> but this is easier to type in Razor.
    /// </summary>
    int ToInt(object value, int fallback = default);

    /// <summary>
    /// Convert any object safely to a Guid
    /// This does the same as <see cref="To{T}(object)"/> but this is easier to type in Razor.
    /// </summary>
    Guid ToGuid(object value);

    /// <summary>
    /// Convert any object safely to standard guid, or if that fails, return the fallback value.
    /// This does the same as <see cref="To{T}(object)"/> but this is easier to type in Razor.
    /// </summary>
    Guid ToGuid(object value, Guid fallback = default);



    /// <summary>
    /// Convert any object safely to string.
    /// This does the same as <see cref="To{T}(object)"/> but this is easier to type in Razor.
    /// </summary>
    string ToString(object value);

    /// <summary>
    /// Convert any object safely to string - or if that fails, return the fallback value.
    /// 
    /// This does **NOT** do the same as <see cref="To{T}(object, NoParamOrder, T)"/>.
    /// In the standard implementation would only give you the fallback, if conversion failed.
    /// But this ToString will also give you the fallback, if the result is null. 
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <param name="fallback">Fallback in case conversion fails or result is null</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallbackOnNull">Determine that nulls should also fallback, default is `true`</param>
    string ToString(object value,
        string fallback = default,
        NoParamOrder noParamOrder = default,
        bool fallbackOnNull = true);

    /// <summary>
    /// Convert any object safely to string to put into source code like HTML-attributes, inline-JavaScript or similar.
    /// This is usually used to ensure numbers, booleans and dates are in a format which works.
    /// Especially useful when giving data to a JavaScript, Json-Fragment or an Html Attribute.
    ///
    /// * booleans will be `true` or `false` (not `True` or `False`)
    /// * numbers will have a . notation and never a comma (like in de-DE cultures)
    /// * dates will convert to ISO format without time zone
    /// </summary>
    string ForCode(object value);

    /// <summary>
    /// Same as <see cref="ForCode(object)"/>, but with fallback, in case the conversion fails.
    /// </summary>
    /// <returns></returns>
    string ForCode(object value, string fallback = default);

    /// <summary>
    /// Sub-Service to convert JSON
    /// </summary>
    IJsonService Json { get; }

    #region Invisible Converts for backward compatibility

    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    int ToInt32(object value);

    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    float ToSingle(object value);

    #endregion

}