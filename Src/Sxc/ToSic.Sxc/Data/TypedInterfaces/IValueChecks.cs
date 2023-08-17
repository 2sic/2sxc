using ToSic.Lib.Documentation;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public interface IValueChecks
    {
        /// <summary>
        /// Check if this typed object has a property of this specified name, and has real data.
        /// The opposite version of this is `IsEmpty(...)`
        /// 
        /// > [!IMPORTANT]
        /// > This method is optimized for use in Razor-like scenarios.
        /// > It's behavior is super-useful but maybe not always expected.
        /// >
        /// > * If the value is a string, and is empty or only contains whitespace (even `&amp;nbsp;`) it is regarded as empty.
        /// > * If the returned value is an empty _list_ (eg. a field containing relationships, without any items in it) it is regarded as empty.
        ///
        /// If you need a different kind of check, just `.Get(...)` the value and perform the checks in your code.
        /// </summary>
        /// <param name="name">the property name like `Image`; some objects also support path to sub-property like `Author.Name`</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <returns>`true` if the property exists and has a real value. If it would return an empty list, it will also return `false`</returns>
        /// <remarks>Adding in 16.03 (WIP)</remarks>
        ///// >   You can change this behavior by changing the `blankIs` attribute.
        ///// <param name="blankIs">
        ///// Change how blank **strings** (empty, whitespace, html-whitespace like `&amp;nbsp;`) are treated.
        ///// `true` means that empty and whitespace strings return `true`,
        ///// `false` means every whitespace incl. empty strings return `false`.
        ///// </param>
        bool IsNotEmpty(string name, string noParamOrder = Protector);//, bool? blankIs = default);

        /// <summary>
        /// Check if this typed object has a property of this specified name, and has real data.
        /// The opposite version of this is `IsNotEmpty(...)`
        ///
        /// > [!IMPORTANT]
        /// > This method is optimized for use in Razor-like scenarios.
        /// > It's behavior is super-useful but maybe not always expected.
        /// >
        /// > * If the value is a string, and is empty or only contains whitespace (even `&amp;nbsp;`) it is regarded as empty.
        /// > * If the returned value is an empty _list_ (eg. a field containing relationships, without any items in it) it is regarded as empty.
        ///
        /// If you need a different kind of check, just `.Get(...)` the value and perform the checks in your code.
        /// </summary>
        /// <param name="name">the property name like `Image`; some objects also support path to sub-property like `Author.Name`</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <returns>`true` if the property exists and has a real value. If it would return an empty list, it will also return `false`</returns>
        /// <remarks>Adding in 16.03 (WIP)</remarks>
        ///// >   You can change this behavior by changing the `blankIs` attribute.
        ///// <param name="blankIs">
        ///// Change how blank **strings** (empty, whitespace, html-whitespace like `&amp;nbsp;`) are treated.
        ///// `true` means that empty and whitespace strings return `true`,
        ///// `false` means every whitespace incl. empty strings return `false`.
        ///// </param>
        bool IsEmpty(string name, string noParamOrder = Protector); //, bool? blankIs = default);

    }
}
