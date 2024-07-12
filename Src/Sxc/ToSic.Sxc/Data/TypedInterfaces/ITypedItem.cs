using System.Text.Json.Serialization;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Data.Internal.Convert;

namespace ToSic.Sxc.Data;

/// <summary>
/// A typed object to access data.
/// Previously Razor code always used `dynamic` <see cref="IDynamicEntity"/> objects.
/// This had some disadvantages when working with LINQ or advanced toolbars.
/// </summary>
/// <remarks>
/// Introduced in 2sxc 16.01
/// </remarks>
[PublicApi]
[JsonConverter(typeof(DynamicJsonConverter))]
public partial interface ITypedItem: ITyped, ICanBeEntity, ICanBeItem, IEquatable<ITypedItem>
{
    /// <summary>
    /// A dynamic accessor for properties, to quickly get values when you don't care about type safety.
    /// This is _often_ (but not always) a <see cref="IDynamicEntity"/>.
    ///
    /// Example: `Dyn.FirstName` might just work - and return the first name or `null` if not found.
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("This should be avoided")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [JsonIgnore] // prevent serialization as it's not a normal property
    dynamic Dyn { get; }

    /// <summary>
    /// The presentation item or `null` if it doesn't exist.
    /// </summary>
    [JsonIgnore] // prevent serialization as it's not a normal property
    ITypedItem Presentation { get; }

    /// <summary>
    /// Metadata of the current item, with special features.
    /// </summary>
    /// <remarks>
    /// Added in 16.02
    /// </remarks>
    [JsonIgnore] // prevent serialization as it's not a normal property
    IMetadata Metadata { get; }

    /// <summary>
    /// Get a special info-object describing a specific field in this item.
    /// This is a rich object used by other operations which need a lot of context about the item and the field.
    /// </summary>
    /// <param name="name">Name of the property</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="required">throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <returns></returns>
    IField Field(string name,
        NoParamOrder noParamOrder = default,
        bool? required = default);

}