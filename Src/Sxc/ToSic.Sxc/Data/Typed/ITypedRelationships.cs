using System.Collections.Generic;
using ToSic.Lib.Documentation;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public interface ITypedRelationships
    {
        /// <summary>
        /// A single item from a field.
        /// If the field doesn't exist or is empty, will return null.
        /// </summary>
        /// <param name="name">Name of the field</param>
        /// <returns></returns>
        ITypedItem Child(string name);

        /// <summary>
        /// A **typed** list of sub-items. Important for LINQ style querying or just
        /// working with various lists. Note that for getting child items of this item you
        /// can just use the properties, like content.Authors. <br/>
        /// But using Children("Authors", typeName) gives you the ability to restrict to a type.  <br/>
        /// Please check the tutorials on 2sxc.org/dnn-tutorials/ for more info. 
        /// </summary>
        /// <param name="type">Optional type filter - would only return items of this type. </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="name">Optional field filter - would only return items that point to the current item in a specific field name.</param>
        /// <returns>A list of all items pointing here (filtered), converted to DynamicEntity for convenience.</returns>
        /// <remarks>Note that the parameter-order is reversed to the Parents()</remarks>
        IEnumerable<ITypedItem> Children(string name = default, string noParamOrder = Protector, string type = default);

        /// <summary>
        /// A **typed** list of entities which point to this item. Important for LINQ style querying or just
        /// working with various lists. Note that for getting child items of this item you
        /// can just use the properties, like content.Authors. <br/>
        /// Please check the tutorials on 2sxc.org/dnn-tutorials/ for more info. 
        /// </summary>
        /// <param name="type">Optional type filter - would only return items of this type. </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="field">Optional field filter - would only return items that point to the current item in a specific field name.</param>
        /// <returns>A list of all items pointing here (filtered), converted to DynamicEntity for convenience.</returns>
        /// <remarks>Note that the parameter-order is reversed to the Children()</remarks>
        IEnumerable<ITypedItem> Parents(string type = default, string noParamOrder = Protector, string field = default);

    }
}
