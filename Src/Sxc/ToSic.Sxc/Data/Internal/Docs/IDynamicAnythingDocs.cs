

// ReSharper disable UnusedMember.Global
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace ToSic.Sxc.Data.Internal.Docs;
/* IMPORTANT: These are just fake properties for documentation - Keep in Sync between IDynamicEntity and IDynamicStack */

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IDynamicAnythingDocs
{
    /// <summary>
    /// A Dynamic Entity always contains an item of your data - a book, person, blog-post or a piece of content.
    /// Since the object is dynamic, you can just use `.IsFemale` or whatever other property your item has.
    /// It is treated as a `dynamic` so you can just output it, or cast it to the expected type.
    /// </summary>
    dynamic AnyProperty { get; }
    
}