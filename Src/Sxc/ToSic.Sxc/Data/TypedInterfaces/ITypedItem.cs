using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// A typed object to access data.
    /// Previously Razor code always used `dynamic` <see cref="IDynamicEntity"/> objects.
    /// This had some disadvantages when working with LINQ or advanced toolbars.
    /// </summary>
    /// <remarks>
    /// Introduced in 2sxc 16.01
    /// </remarks>
    [PublicApi]
    public partial interface ITypedItem: ITyped, ICanBeEntity, ICanBeItem
    {

        /// <summary>
        /// The presentation item or `null` if it doesn't exist.
        /// </summary>
        ITypedItem Presentation { get; }

        /// <summary>
        /// Metadata of the current item, with special features.
        /// </summary>
        /// <remarks>
        /// Added in 16.02
        /// </remarks>
        IMetadata Metadata { get; }

        /// <summary>
        /// Get a special info-object describing a specific field in this item.
        /// This is a rich object used by other operations which need a lot of context about the item and the field.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="required">throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.TypedRequired)</param>
        /// <returns></returns>
        IField Field(string name,
            string noParamOrder = Protector,
            bool? required = default);



    }
}