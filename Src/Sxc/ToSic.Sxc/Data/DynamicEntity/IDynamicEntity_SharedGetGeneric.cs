namespace ToSic.Sxc.Data
{
    public partial interface IDynamicEntity
    {
        /*
         IMPORTANT
         KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityBase, IDynamicStack, ITypedItem
         Master copy should always be IDynamicEntity
        */


        /// <summary>
        /// Get a value using the name - and cast it to the expected strong type.
        /// For example to get an int even though it's stored as decimal.
        /// </summary>
        /// <typeparam name="TValue">The expected type, like `string`, `int`, etc.</typeparam>
        /// <param name="name">the property name like `Image` - or path like `Author.Name` (new v15)</param>
        /// <returns>The typed value, or the `default` like `null` or `0` if casting isn't possible.</returns>
        /// <remarks>Added in v15</remarks>
        new TValue Get<TValue>(string name);

        /// <summary>
        /// Get a value using the name - and cast it to the expected strong type.
        /// For example to get an int even though it's stored as decimal.
        ///
        /// Since the parameter `fallback` determines the type `TValue` you can just write this like
        /// `Content.Get("Title", fallback: "no title")
        /// </summary>
        /// <typeparam name="TValue">
        /// The expected type, like `string`, `int`, etc.
        /// Note that you don't need to specify it, if you specify the `fallback` property.
        /// </typeparam>
        /// <param name="name">the property name like `Image` - or path like `Author.Name` (new v15)</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">the fallback value to provide if not found</param>
        /// <returns>The typed value, or the `default` like `null` or `0` if casting isn't possible.</returns>
        /// <remarks>Added in v15</remarks>
        new TValue Get<TValue>(string name,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            string noParamOrder = Eav.Parameters.Protector,
            TValue fallback = default);
    }
}
