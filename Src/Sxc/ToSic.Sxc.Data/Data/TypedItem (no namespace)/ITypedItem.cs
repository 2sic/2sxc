using System.Text.Json.Serialization;
using ToSic.Sxc.Data.Sys.Json;

namespace ToSic.Sxc.Data;

/// <summary>
/// A typed item to access <see cref="IEntity"/> data in a strongly typed way.
/// </summary>
/// <remarks>
/// Previously Razor code always used `dynamic` <see cref="IDynamicEntity"/> objects.
/// This had some disadvantages when working with LINQ or advanced toolbars.
/// 
/// History: Introduced in 2sxc 16.01
/// </remarks>
[PublicApi]
[JsonConverter(typeof(DynamicJsonConverter))]
public partial interface ITypedItem: ITyped, ICanBeEntity, ICanBeItem, IEquatable<ITypedItem>
{
    /// <summary>
    /// The presentation item or `null` if it doesn't exist.
    /// </summary>
    [JsonIgnore] // prevent serialization as it's not a normal property
    ITypedItem? Presentation { get; }

    /// <summary>
    /// Metadata of the current item, with special features.
    /// </summary>
    /// <remarks>
    /// Added in 16.02
    /// </remarks>
    [JsonIgnore] // prevent serialization as it's not a normal property
    ITypedMetadata Metadata { get; }

    /// <summary>
    /// Get a special info-object describing a specific field in this item.
    /// This is a rich object used by other operations which need a lot of context about the item and the field.
    /// </summary>
    /// <param name="name">Name of the property</param>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="required">throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <returns></returns>
    IField? Field(string name,
        NoParamOrder npo = default,
        bool? required = default);

}