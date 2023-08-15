using System.Collections.Generic;
using ToSic.Lib.Documentation;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public interface IHasKeys
    {
        /// <summary>
        /// Check if this typed object has a property of this specified name.
        /// It's case insensitive.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <remarks>Adding in 16.03 (WIP)</remarks>
        bool ContainsKey(string name);

        /// <summary>
        /// Check if this typed object has a property of this specified name, and has real data.
        /// It's case insensitive.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>`true` if the property exists and has a real value. If it would return an empty list, it will also return `false`</returns>
        /// <remarks>Adding in 16.03 (WIP)</remarks>
        bool ContainsData(string name);


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
