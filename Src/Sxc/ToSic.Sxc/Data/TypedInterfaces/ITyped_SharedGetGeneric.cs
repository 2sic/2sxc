using System.Collections.Generic;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    public partial interface ITyped //: IHasKeys
    {
        /// <inheritdoc cref="IHasKeys.ContainsKey"/>
        bool ContainsKey(string name);

        /// <inheritdoc cref="IHasKeys.ContainsData"/>
        bool ContainsData(string name, string noParamOrder = Protector, bool? blankIs = default);

        /// <inheritdoc cref="IHasKeys.Keys"/>
        IEnumerable<string> Keys(string noParamOrder = Protector, IEnumerable<string> only = default);


        /// <summary>
        /// Get a property.
        /// </summary>
        /// <param name="name">the property name like `Image` - or path to sub-property like `Author.Name` (new v15)</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="required">throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.TypedRequired)</param>
        /// <returns>The result if found or null; or error if the object is in strict mode</returns>
        object Get(string name,
            string noParamOrder = Protector,
            bool? required = default);

        // 2023-08-04 2dm removed/disabled, not useful as we should always be able to specify strict
        ///// <summary>
        ///// Get a value using the name - and cast it to the expected strong type.
        ///// For example to get an int even though it's stored as decimal.
        ///// </summary>
        ///// <typeparam name="TValue">The expected type, like `string`, `int`, etc.</typeparam>
        ///// <param name="name">the property name like `Image` - or path like `Author.Name` (new v15)</param>
        ///// <returns>The typed value, or the `default` like `null` or `0` if casting isn't possible.</returns>
        ///// <remarks>Added in v15</remarks>
        //TValue Get<TValue>(string name);

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
        /// <param name="required">throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.TypedRequired)</param>
        /// <returns>The typed value, or the `default` like `null` or `0` if casting isn't possible.</returns>
        /// <remarks>Added in v15</remarks>
        TValue Get<TValue>(string name,
            string noParamOrder = Protector,
            TValue fallback = default,
            bool? required = default);


    }
}
