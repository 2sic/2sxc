using ToSic.Eav.Metadata;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Data.Sys.Field;

namespace ToSic.Sxc.Data;

/// <summary>
/// This describes a field-property of an item/entity. 
/// </summary>
/// <remarks>
/// It's used for APIs which can need to know more about the field holding an item, like:
///
/// - The field name and parent reference
/// - The values in raw and converted
/// - Any metadata of the field
/// 
/// History
/// 
/// * Created in v13.10, originally as `IDynamicField`
/// * In v16.02 renamed `IField` as it's not dynamic, and published
/// * In 16.02 changed types of `Value` and `Raw` to `object` - previously `dynamic`
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("This is just FYI so you see how it works; you shouldn't use any of these properties in your code")]
public interface IField: IHasLink, IHasMetadata
{
    /// <summary>
    /// The field name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The parent object of this field
    /// </summary>
    ITypedItem Parent { get; }

    /// <summary>
    /// The raw value of the field, without any modifications.
    /// If the value is `file:22` then Raw will also return `file:22`.
    /// To get the value as a link, use <see cref="Value"/>
    /// </summary>
    [PrivateApi("Was public till 16.03, but don't think it should be surfaced...")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    object? Raw { get; }

    /// <summary>
    /// The value of the field with modifications.
    /// For example, `file:22` would be converted to the real link.
    /// To get the raw value, use <see cref="Raw"/>
    /// </summary>
    [PrivateApi("Was public till 16.03, but don't think it should be surfaced...")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    object? Value { get; }

    ///// <summary>
    ///// Metadata of the thing in the field - if it has such metadata.
    /////
    ///// The object will never be null, but it can of course not have any data if there is no metadata. 
    ///// </summary>
    //new IMetadata Metadata { get; }
}