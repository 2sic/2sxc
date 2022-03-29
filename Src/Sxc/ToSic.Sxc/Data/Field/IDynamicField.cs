using ToSic.Eav.Apps.Decorators;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// # BETA
    ///
    /// Work in progress.
    ///
    /// This is an interface which should describe a field.
    /// It's important for APIs which can need to know more about the field holding an item, like:
    ///
    /// - The field name
    /// - Any metadata of the field
    /// </summary>
    [WorkInProgressApi("Work in progress, to be finalized ca. v13.10")]
    public interface IDynamicField: IHasLink
    {
        /// <summary>
        /// The field name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The parent object holding this field
        /// </summary>
        IDynamicEntity Parent { get; }

        /// <summary>
        /// The raw value of the field, without any modifications
        /// </summary>
        dynamic Raw { get; }

        /// <summary>
        /// The value of the field with modifications - for example, `file:22` would be converted to the real link
        /// </summary>
        dynamic Value { get; }

        [PrivateApi]
        ImageDecorator ImageDecoratorOrNull { get; }
    }
}
