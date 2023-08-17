using System.Collections.Generic;
using ToSic.Lib.Documentation;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public interface IHasKeys: IValueChecks
    {
        /// <summary>
        /// Check if this typed object has a property of this specified name.
        /// It's case insensitive.
        /// </summary>
        /// <param name="name">the property name like `Image`; some objects also support path to sub-property like `Author.Name`</param>
        /// <returns></returns>
        /// <remarks>Adding in 16.03 (WIP)</remarks>
        bool ContainsKey(string name);

        ///// <summary>
        ///// Check if this typed object has a property of this specified name, and has real data.
        /////
        ///// > [!IMPORTANT]
        ///// > This method is optimized for use in Razor-like scenarios.
        ///// > It may have some behavior that is super-useful but maybe not always expected.
        ///// >
        ///// > * If the value is a string, and is empty or only contains whitespace (even `&amp;nbsp;`) it will still return `false` - you can change this behavior by changing the `blankIs` attribute.
        ///// > * If the returned value is an empty list (eg. a field containing relationships, without any items in it) it returns `false`.
        ///// </summary>
        ///// <param name="name">the property name like `Image`; some objects also support path to sub-property like `Author.Name`</param>
        ///// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        ///// <param name="blankIs">Change how blank strings (empty, whitespace, html-whitespace) is treated.
        ///// `true` means that every whitespace inkl. empty strings return `true`,
        ///// `false` (default) means every whitespace incl. empty strings return `false`.
        ///// </param>
        ///// <returns>`true` if the property exists and has a real value. If it would return an empty list, it will also return `false`</returns>
        ///// <remarks>Adding in 16.03 (WIP)</remarks>
        //bool ContainsData(string name, string noParamOrder = Protector, bool? blankIs = default);


        /// <summary>
        /// Get all the keys available in this Model (all the parameters passed in).
        /// This is used to sometimes run early checks if all the expected parameters have been provided.
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="only">Only return the keys specified here. Typical use: `only: new [] { "Key1", "Key2" }`</param>
        /// <returns></returns>
        /// <remarks>Added in 16.03</remarks>
        IEnumerable<string> Keys(string noParamOrder = Protector, IEnumerable<string> only = default);

    }
}
