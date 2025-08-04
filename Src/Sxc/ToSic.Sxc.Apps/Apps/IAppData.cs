using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Apps;

/// <summary>
/// An App-DataSource which also provides direct commands to edit/update/delete data.
/// </summary>
[PublicApi]
public interface IAppData: IDataSource, IMetadataSource
{
    /// <summary>
    /// Create a new entity in the storage.
    /// </summary>
    /// <param name="contentTypeName">The type name</param>
    /// <param name="values">a dictionary of values to be stored</param>
    /// <param name="userName">the current username - will be logged as the author</param>
    /// <param name="target">information if this new item is to be metadata for something</param>
    /// <remarks>
    /// Changed in 2sxc 10.30 - now returns the id of the created items
    /// </remarks>
    IEntity Create(string contentTypeName, Dictionary<string, object> values, string? userName = null, ITarget? target = null);

    /// <summary>
    /// Create a bunch of new entities in one single call (much faster, because cache doesn't need to repopulate in the meantime).
    /// </summary>
    /// <param name="contentTypeName">The type name</param>
    /// <param name="multiValues">many dictionaries, each will become an own item when stored</param>
    /// <param name="userName">the current username - will be logged as the author</param>
    /// <remarks>
    /// You can't create items which are metadata with this, for that, please use the Create-one overload <br/>
    /// Changed in 2sxc 10.30 - now returns the id of the created items
    /// </remarks>
    IEnumerable<IEntity> Create(string contentTypeName, IEnumerable<Dictionary<string, object>> multiValues, string? userName = null);

    /// <summary>
    /// Update an existing item.
    /// </summary>
    /// <param name="entityId">The item ID</param>
    /// <param name="values">a dictionary of values to be updated</param>
    /// <param name="userName">the current username - will be logged as the author of the change</param>
    void Update(int entityId, Dictionary<string, object> values, string? userName = null);

    /// <summary>
    /// Delete an existing item
    /// </summary>
    /// <param name="entityId">The item ID</param>
    /// <param name="userName">the current username - will be logged as the author of the change</param>
    void Delete(int entityId, string? userName = null);

    /// <summary>
    /// Get metadata of TargetType.Custom - which is the most common way your code will need Metadata.
    /// </summary>
    /// <typeparam name="TKey">Key data type</typeparam>
    /// <param name="key">The target identifier - a number, string or Guid</param>
    /// <param name="contentTypeName">Optional name of Content-Type, if you only want items of a specific type</param>
    /// <returns></returns>
    IEnumerable<IEntity> GetCustomMetadata<TKey>(TKey key, string? contentTypeName = null);


    /// <summary>
    /// All content types of the app.
    /// </summary>
    /// <remarks>
    /// * Added v20
    /// * Implemented as a method, so later we can apply filters etc. as additional parameters
    /// </remarks>
    IEnumerable<IContentType> GetContentTypes();

    /// <summary>
    /// Get a single content type by name (display name or NameId).
    /// </summary>
    /// <param name="name">the name, either the normal name or the NameId which looks like a GUID</param>
    /// <remarks>
    /// Added v20
    /// </remarks>
    IContentType? GetContentType(string name);

}