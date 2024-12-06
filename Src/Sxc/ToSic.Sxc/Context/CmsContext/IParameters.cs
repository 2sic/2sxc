using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Context;

/// <summary>
/// Collection of url parameters of the current page
///
/// Note: Has a special ToString() implementation, which gives you the parameters for re-use in other scenarios.
/// 
/// 🪒 In [Dynamic Razor](xref:Custom.Hybrid.Razor14) it's found on `CmsContext.Page.Parameters`  
/// 🪒 In [Typed Razor](xref:Custom.Hybrid.RazorTyped) it's found on `MyPage.Parameters`
/// </summary>
/// <remarks>
/// * uses the [](xref:NetCode.Conventions.Functional)
/// * Added typed accessors such as `Int(...)` etc. in v16.03 implementing <see cref="ITyped"/>
/// * Made order of parameters automatically sort in 18.06 because of crawler-load issues
/// * Added `Prioritize` in v18.06
/// </remarks>
[PublicApi]
public interface IParameters: IReadOnlyDictionary<string, string>, ITyped
{

    #region Get (new v15.04)

    /// <summary>
    /// Get a parameter.
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.Page.Parameters.Get("SortOrder")`  
    /// 🪒 Use in Typed Razor: `MyPage.Parameters.Get("SortOrder")`
    /// </summary>
    /// <param name="name">the key/name in the url</param>
    /// <returns>a string or null</returns>
    /// <remarks>
    /// Added v15.04
    /// </remarks>
    string Get(string name);

    /// <summary>
    /// Get a parameter and convert to the needed type - or return the default.
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.Page.Parameters.Get&lt;int&gt;("id")`  
    /// 🪒 Use in Typed Razor: `MyPage.Parameters.Get&lt;int&gt;("id")`
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="name">Key/name of the parameter</param>
    /// <returns></returns>
    /// <remarks>
    /// Added v15.04
    /// </remarks>
    TValue Get<TValue>(string name);

    /// <summary>
    /// Get a parameter and convert to the needed type - or return the fallback.
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.Page.Parameters.Get("id", fallback: 0)`  
    /// 🪒 Use in Typed Razor: `MyPage.Parameters.Get("SortOrder", fallback: 0)`
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="name">Key/name of the parameter</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">Optional fallback value to use if not found</param>
    /// <returns></returns>
    /// <remarks>
    /// Added v15.04
    /// </remarks>
    // ReSharper disable once MethodOverloadWithOptionalParameter
    TValue Get<TValue>(string name, NoParamOrder noParamOrder = default, TValue fallback = default);

    #endregion

    #region Add / Set / Remove

    /// <summary>
    /// Add another URL parameter and return a new <see cref="IParameters"/>.
    /// If the name/key already exists, it will extend it, add a simple 
    /// Otherwise please use <see cref="Set(string,string)"/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns>A _new_ <see cref="IParameters"/>, the original is not modified.</returns>
    IParameters Add(string key);

    /// <summary>
    /// Add another URL parameter and return a new <see cref="IParameters"/>.
    /// If the name/key already exists, it will extend it, so the parameter will have 2 values.
    /// Otherwise, please use <see cref="Set(string,string)"/>
    /// </summary>
    /// <param name="key">the key</param>
    /// <param name="value">the value</param>
    /// <returns>A _new_ <see cref="IParameters"/>, the original is not modified.</returns>
    IParameters Add(string key, string value);

    /// <summary>
    /// Add another URL parameter and return a new <see cref="IParameters"/>.
    /// If the name/key already exists, it will extend it, so the parameter will have 2 values.
    /// Otherwise, please use <see cref="Set(string,string)"/>
    ///
    /// Note also that this takes an `object` and will do some special conversions.
    /// For example, bool values are lower case `true`|`false`, numbers are culture invariant and dates
    /// are treated as is with time removed if it has no time. 
    /// </summary>
    /// <param name="key">the key</param>
    /// <param name="value">object! value</param>
    /// <returns>A _new_ <see cref="IParameters"/>, the original is not modified.</returns>
    /// <remarks>Added in v15.0</remarks>
    IParameters Add(string key, object value);

    /// <summary>
    /// Add another URL parameter and return a new <see cref="IParameters"/>.
    /// If the name/key already exists, it will just overwrite it.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns>A _new_ <see cref="IParameters"/>, the original is not modified.</returns>
    IParameters Set(string name, string value);

    /// <summary>
    /// Add another URL parameter and return a new <see cref="IParameters"/>.
    /// If the name/key already exists, it will just overwrite it.
    ///
    /// Note also that this takes an `object` and will do some special conversions.
    /// For example, bool values are lower case `true`|`false`, numbers are culture invariant and dates
    /// are treated as is with time removed if it has no time. 
    /// </summary>
    /// <param name="name">the key</param>
    /// <param name="value">object! value</param>
    /// <returns>A _new_ <see cref="IParameters"/>, the original is not modified.</returns>
    /// <remarks>Added in v15.0</remarks>
    IParameters Set(string name, object value);

    /// <summary>
    /// Add another URL parameter and return a new <see cref="IParameters"/>.
    /// If the name/key already exists, it will just overwrite it.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>A _new_ <see cref="IParameters"/>, the original is not modified.</returns>
    IParameters Set(string name);

    /// <summary>
    /// Remove a parameter and return a new <see cref="IParameters"/>.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>A _new_ <see cref="IParameters"/>, the original is not modified.</returns>
    IParameters Remove(string name);

    ///// <summary>
    ///// Remove a parameter **value** and return a new <see cref="IParameters"/>.
    ///// This only removes a specific value, for example if you start with `id=27&amp;id=42` and remove `id=27`, then the result will be `id=42`.
    ///// </summary>
    ///// <param name="name"></param>
    ///// <param name="value"></param>
    ///// <remarks>Added in v17.01</remarks>
    ///// <returns>A _new_ <see cref="IParameters"/>, the original is not modified.</returns>
    //IParameters Remove(string name, string value);

    /// <summary>
    /// Remove a parameter **value** and return a new <see cref="IParameters"/>.
    /// This only removes a specific value, for example if you start with `id=27&amp;id=42` and remove `id=27`, then the result will be `id=42`.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns>A _new_ <see cref="IParameters"/>, the original is not modified.</returns>
    /// <remarks>Added in v17.01</remarks>
    IParameters Remove(string name, object value);

    #endregion

    #region Toggle (new v17)

    ///// <summary>
    ///// Toggle a parameter value and return a new <see cref="IParameters"/>.
    /////
    ///// This means that if the parameter was previously set with the same value, it will be un-set, otherwise it will be added.
    ///// </summary>
    ///// <param name="name"></param>
    ///// <param name="value"></param>
    ///// <remarks>Added in v17.01</remarks>
    //IParameters Toggle(string name, string value);

    /// <summary>
    /// Toggle a parameter value and return a new <see cref="IParameters"/>.
    ///
    /// This means that if the parameter was previously set with the same value, it will be un-set, otherwise it will be added.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns>A _new_ <see cref="IParameters"/>, the original is not modified.</returns>
    /// <remarks>Added in v17.01</remarks>
    IParameters Toggle(string name, object value);

    /// <summary>
    /// Filter all parameters to only keep the keys listed in `names`.
    /// </summary>
    /// <param name="names">one or more names to keep, comma-separated.</param>
    /// <returns>A _new_ <see cref="IParameters"/>, the original is not modified.</returns>
    /// <remarks>Added in v17.01</remarks>
    public IParameters Filter(string names);

    #endregion

    #region ToString to easily create url params from this object

    /// <summary>
    /// ToString() is specially implemented, to give you the parameters again as they were originally given on the page.
    /// </summary>
    /// <returns></returns>
    new string ToString();

    #endregion

    #region Handle duplicate interface methods

    /// <inheritdoc cref="IHasKeys.ContainsKey"/>
    new bool ContainsKey(string name);
    // ^^^ this is added, because both Dictionary and ITyped have this method, so it could be unclear

    #endregion

    /// <summary>
    /// Prioritize the order of parameters.
    /// This allows you to order the parameters in a certain way, which can be important for some systems.
    ///
    /// Remember:
    /// 1. If a parameter doesn't exist, it still won't appear in the list
    /// 2. If you order the parameters, this can have an unexpected effect on the amount of URLs you generate, possibly causing high server load when crawlers visit.
    /// </summary>
    /// <remarks>
    /// New in v18.06.
    /// </remarks>
    /// <param name="fields">CSV of names to prioritize, in the specified order</param>
    /// <returns></returns>
    IParameters Prioritize(string fields = default);

    /// <summary>
    /// Flush all parameters and start anew.
    /// Note that it does preserve other settings like prioritization.
    /// </summary>
    /// <remarks>
    /// New v18.06
    /// </remarks>
    /// <returns></returns>
    IParameters Flush();
}