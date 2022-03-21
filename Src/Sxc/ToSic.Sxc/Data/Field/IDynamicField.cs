using ToSic.Eav.Apps.Decorators;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// # BETA
    ///
    /// Work in progress.
    ///
    /// This is an interface which should describe a field. It's important for APIs which can need to know more about the field holding an item. 
    /// </summary>
    public interface IDynamicField
    {
        /// <summary>
        /// The field name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The parent object holding this field
        /// </summary>
        IDynamicEntity Parent { get; }

        [PrivateApi]
        ImageDecorator ImageDecoratorOrNull();
    }
}
