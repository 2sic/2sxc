using System.Collections.Generic;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// Collection of url parameters of the current page
    ///
    /// Has a special ToString() implementation, which gives you the parameters for re-use in other scenarios...?
    /// </summary>
    /// <remarks>
    /// * uses the [](xref:NetCode.Conventions.Functional)
    /// </remarks>
    [PublicApi]
    public interface IParameters: IReadOnlyDictionary<string, string>
    {
        /// <summary>
        /// ToString is especially implemented, to give you the parameters again as they were originally given on the page.
        /// </summary>
        /// <returns></returns>
        string ToString();

        #region Get (new v15.04)

        /// <summary>
        /// Get a parameter.
        /// </summary>
        /// <param name="name">the key/name in the url</param>
        /// <returns>a string or null</returns>
        /// <remarks>
        /// Added v15.04
        /// </remarks>
        string Get(string name);

        /// <summary>
        /// Get a parameter and convert to the needed type - or return the default.
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
        TValue Get<TValue>(string name, string noParamOrder = Eav.Parameters.Protector, TValue fallback = default);

        #endregion

        /// <summary>
        /// Add another URL parameter and return a new <see cref="IParameters"/>.
        /// If the name/key already exists, it will extend it, add a simple 
        /// Otherwise please use <see cref="Set(string,string)"/>
        /// 
        /// _Important: this does not change the current object, it returns a new object._
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IParameters Add(string key);

        /// <summary>
        /// Add another URL parameter and return a new <see cref="IParameters"/>.
        /// If the name/key already exists, it will extend it, so the parameter will have 2 values.
        /// Otherwise please use <see cref="Set(string,string)"/>
        /// 
        /// _Important: this does not change the current object, it returns a new object._
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">the value</param>
        /// <returns>A new <see cref="IParameters"/> object</returns>
        IParameters Add(string key, string value);

        /// <summary>
        /// Add another URL parameter and return a new <see cref="IParameters"/>.
        /// If the name/key already exists, it will extend it, so the parameter will have 2 values.
        /// Otherwise please use <see cref="Set(string,string)"/>
        /// 
        /// _Important: this does not change the current object, it returns a new object._
        ///
        /// Note also that this takes an `object` and will do some special conversions.
        /// For example, bool values are lower case `true`|`false`, numbers are culture invariant and dates
        /// are treated as is with time removed if it has no time. 
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">object! value</param>
        /// <returns>A new <see cref="IParameters"/> object</returns>
        /// <remarks>Added in v15.0</remarks>
        IParameters Add(string key, object value);

        /// <summary>
        /// Add another URL parameter and return a new <see cref="IParameters"/>.
        /// If the name/key already exists, it will just overwrite it.
        /// 
        /// _Important: this does not change the current object, it returns a new object._
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns>A new <see cref="IParameters"/> object</returns>
        IParameters Set(string name, string value);

        /// <summary>
        /// Add another URL parameter and return a new <see cref="IParameters"/>.
        /// If the name/key already exists, it will just overwrite it.
        /// 
        /// _Important: this does not change the current object, it returns a new object._
        ///
        /// Note also that this takes an `object` and will do some special conversions.
        /// For example, bool values are lower case `true`|`false`, numbers are culture invariant and dates
        /// are treated as is with time removed if it has no time. 
        /// </summary>
        /// <param name="name">the key</param>
        /// <param name="value">object! value</param>
        /// <returns>A new <see cref="IParameters"/> object</returns>
        /// <remarks>Added in v15.0</remarks>
        IParameters Set(string name, object value);

        /// <summary>
        /// Add another URL parameter and return a new <see cref="IParameters"/>.
        /// If the name/key already exists, it will just overwrite it.
        /// 
        /// _Important: this does not change the current object, it returns a new object._
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IParameters Set(string name);

        /// <summary>
        /// Remove a parameter and return a new <see cref="IParameters"/>.
        ///
        /// _Important: this does not change the current object, it returns a new object._
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IParameters Remove(string name);
    }
}
