using ToSic.Sxc.Data;


namespace ToSic.Sxc.Services;

/// <summary>
/// Service on [`Kit.Json`](xref:ToSic.Sxc.Services.ServiceKit16.Json) to serialize/restore JSON.
/// </summary>
/// <remarks>
/// It works for 2sxc/EAV data but can be used for any data which can be serialized/deserialized.
/// Since it's a data-operation, we keep it in this namespace, even if most other things in this namespace are 2sxc-data objects.
///
/// *Important* This is simple object-string conversion.
/// It doesn't change entity objects to be serializable.
/// For that you should use the [](xref:ToSic.Eav.DataFormats.EavLight.IConvertToEavLight) which returns an object that can then be serialized. 
/// Internally it uses System.Text.Json and preserves the case of keys.
///
/// History
/// 
/// * Introduced in 2sxc 12.05.
///
/// </remarks>
[PublicApi]
public interface IJsonService
{
    /// <summary>
    /// Convert an object to JSON.
    /// 
    /// If you need to add the JSON to HTML of a page, make sure you also use `Html.Raw(...)`, otherwise it will be encoded and not usable in JavaScript.
    /// </summary>
    /// <param name="item">The object to serialize</param>
    string ToJson(object item);

    /// <summary>
    /// Convert an object to JSON - using nicer output / indentation.
    /// 
    /// If you need to add the JSON to HTML of a page, make sure you also use `Html.Raw(...)`, otherwise it will be encoded and not usable in JavaScript.
    /// </summary>
    /// <param name="item">The object to serialize</param>
    /// <param name="indentation">How much to indent the json - we recommend 4. As of now, it will always use 4, no matter what you set (see remarks)</param>
    /// <remarks>
    /// Added in 2sxc 12.11
    ///
    /// But as of 2sxc 12.11 we're still using an old Newtonsoft, so we cannot really control the indentation depth.
    /// If you call this, it will always indent using 4 spaces. In a future release we'll probably use a newer Newtonsoft with which we can then use the indentation as needed.
    /// </remarks>
    string ToJson(object item, int indentation);

    /// <summary>
    /// Convert a JSON to a typed object. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    T? To<T>(string json);

    /// <summary>
    /// Convert a json to an anonymous object.
    /// This is a very technical thing to do, so only use it if you know why you're doing this.
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    object? ToObject(string json);

    /// <summary>
    /// Creates a <see cref="ITyped"/> object from a json string.
    ///
    /// > [!IMPORTANT]
    /// > This only works on json strings which return an object.
    /// > If you pass in a simple json such as `27` or `"hello"` or an array like `[1, 2, 3]` it will throw an error.
    /// > For arrays, use <see cref="ToTypedList"/>.
    /// </summary>
    /// <param name="json">The string containing json</param>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">
    /// Alternate string to use, if the original json can't parse.
    /// Can also be null or the word "error" if you would prefer an error to be thrown.
    /// </param>
    /// <param name="propsRequired">make the resulting object [strict](xref:NetCode.Conventions.PropertiesRequired), default `true`</param>
    /// <returns>A dynamic object representing the original json.
    /// If it can't be parsed, it will parse the fallback, which by default is an empty dynamic object.
    /// If you provide null for the fallback, then you will get null back.
    /// </returns>
    /// <remarks>
    /// New in 16.02
    /// </remarks>
    [PublicApi]
    ITyped? ToTyped(string json, NoParamOrder npo = default, string? fallback = default, bool? propsRequired = default);

    /// <summary>
    /// Creates a list of <see cref="ITyped"/> wrappers around a json string containing an array of objects.
    ///
    /// > [!IMPORTANT]
    /// > This only works on json strings which return an object.
    /// > If you pass in a simple json such as `27` or `"hello"` or an array like `[1, 2, 3]` it will throw an error.
    /// > For arrays, use <see cref="ToTypedList"/>.
    /// </summary>
    /// <param name="json">The string containing json</param>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">
    /// Alternate string to use, if the original json can't parse.
    /// Can also be null or the word "error" if you would prefer an error to be thrown.
    /// </param>
    /// <param name="propsRequired">make the resulting object [strict](xref:NetCode.Conventions.PropertiesRequired), default `true`</param>
    /// <returns>A dynamic object representing the original json.
    /// If it can't be parsed, it will parse the fallback, which by default is an empty dynamic object.
    /// If you provide null for the fallback, then you will get null back.
    /// </returns>
    /// <remarks>
    /// New in 16.04
    /// </remarks>
    IEnumerable<ITyped>? ToTypedList(string json, NoParamOrder npo = default, string? fallback = default,
        bool? propsRequired = default);
}