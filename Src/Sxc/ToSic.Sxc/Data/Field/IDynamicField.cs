using ToSic.Eav.Apps.Decorators;
using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Images;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This describes a field-property of an item/entity. 
    /// It's used for APIs which can need to know more about the field holding an item, like:
    ///
    /// - The field name and parent reference
    /// - The values in raw and converted
    /// - Any metadata of the field
    /// 
    /// </summary>
    /// <remarks>
    /// History: Created in v13.10
    /// </remarks>
    [PublicApi]
    public interface IDynamicField: IHasLink, IHasMetadata
    {
        /// <summary>
        /// The field name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The parent object of this field
        /// </summary>
        IDynamicEntity Parent { get; }

        /// <summary>
        /// The raw value of the field, without any modifications.
        /// If the value is `file:22` then Raw will also return `file:22`.
        /// To get the value as a link, use <see cref="Value"/>
        /// </summary>
        dynamic Raw { get; }

        /// <summary>
        /// The value of the field with modifications.
        /// For example, `file:22` would be converted to the real link.
        /// To get the raw value, use <see cref="Raw"/>
        /// </summary>
        dynamic Value { get; }


        /// <summary>
        /// Metadata of the thing in the field - if it has such metadata.
        ///
        /// The object will never be null, but it can of course not have any data if there is no metadata. 
        /// </summary>
        IDynamicMetadata Metadata { get; }


        [PrivateApi("Internal use only, may change at any time")]
        ImageDecorator ImageDecoratorOrNull { get; }
    }
}
