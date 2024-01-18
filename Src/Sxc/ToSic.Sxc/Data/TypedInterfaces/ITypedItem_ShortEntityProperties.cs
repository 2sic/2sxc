namespace ToSic.Sxc.Data;

partial interface ITypedItem
{
    /// <summary>
    /// The ID of the underlying entity.
    /// Use it for edit-functionality or just to have a unique number for this item.
    /// </summary>
    /// <remarks>If the entity doesn't exist, it will return 0</remarks>
    int Id { get; }

    /// <summary>
    /// The guid of the underlying entity.
    /// </summary>
    /// <remarks>If the entity doesn't exist, it will return an empty guid</remarks>
    Guid Guid { get; }

    /// <summary>
    /// The title of this item. This is always available no matter what the underlying field for the title is. 
    /// </summary>
    /// <returns>
    /// The title of the underlying entity.
    /// In rare cases where no title-field is known, it can be null.
    /// It can also be null if there is no underlying entity. 
    /// </returns>
    /// <remarks>This returns a string which is usually what's expected. In previous versions (before v15) 2sxc it returned an object.</remarks>
    string Title { get; }

    /// <summary>
    /// The Content-Type of the current entity.
    /// </summary>
    IContentType Type { get; }
}