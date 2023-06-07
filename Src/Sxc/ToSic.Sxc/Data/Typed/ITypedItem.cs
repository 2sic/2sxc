using System;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;

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
    public partial interface ITypedItem: ICanBeEntity
    {
        int EntityId { get; }
        Guid EntityGuid { get; }

        /// <summary>
        /// A dynamic accessor for properties, to quickly get values when you don't care about type safety.
        /// This actually is a classic <see cref="IDynamicEntity"/>.
        ///
        /// Example: `Dyn.FirstName` will just work - and return the first name or `null` if not found.
        /// 
        /// </summary>
        dynamic Dyn { get; }

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


        IDynamicField Field(string name);
        object Get(string name);

    }
}