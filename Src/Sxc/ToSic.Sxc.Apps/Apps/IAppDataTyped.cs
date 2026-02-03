using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Apps;

/// <summary>
/// Data object of an App in **Typed** mode
/// </summary>
/// <remarks>Added v17</remarks>
[PublicApi]
public interface IAppDataTyped: IDataSource
{
    #region CRUD

    /// <inheritdoc cref="IAppData.Create(string, Dictionary{string, object}, string, ITarget)"/>
    IEntity Create(string contentTypeName, Dictionary<string, object?> values, string? userName = null, ITarget? target = null);

    /// <inheritdoc cref="IAppData.Create(string, IEnumerable{Dictionary{string, object}}, string)"/>
    IEnumerable<IEntity> Create(string contentTypeName, IEnumerable<Dictionary<string, object?>> multiValues, string? userName = null);

    /// <inheritdoc cref="IAppData.Update"/>
    void Update(int entityId, Dictionary<string, object?> values, string? userName = null);

    /// <inheritdoc cref="IAppData.Delete"/>
    void Delete(int entityId, string? userName = null);

    #endregion

    #region ContentTypes

    /// <summary>
    /// All content types of the app.
    /// </summary>
    /// <remarks>
    /// * Added v17
    /// * Implemented as a method, so later we can apply filters etc. as additional parameters
    /// </remarks>
    IEnumerable<IContentType> GetContentTypes();

    /// <summary>
    /// Get a single content type by name (display name or NameId).
    /// </summary>
    /// <param name="name">the name, either the normal name or the NameId which looks like a GUID</param>
    /// <remarks>Added v17</remarks>
    IContentType? GetContentType(string name);

    #endregion

    #region GetAll, GetOne, GetMany WIP v17.02+

    /// <summary>
    /// Get all data from the app of the specified type.
    /// It will detect the expected Content-Type based on the name of the class used.
    /// 
    /// So in most cases you will not add any parameters except for the type parameter `T`.
    /// This is usually a type of your `AppCode.Data` namespace.
    /// </summary>
    /// <typeparam name="T">The type to get and convert to - usually inheriting `Custom.Data.CustomItem`</typeparam>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="typeName">_optional_ type name which is used as the **stream** name when retrieving the data, as each stream contains entities of one type.</param>
    /// <param name="nullIfNotFound">if set, will return null if the type doesn't exist - default is empty list.</param>
    /// <returns></returns>
    /// <remarks>
    /// Released in v17.03.
    /// </remarks>
    IEnumerable<T>? GetAll<T>(NoParamOrder npo = default, string? typeName = default,
        bool nullIfNotFound = default)
        where T : class, IModelOfData;

    /// <summary>
    /// Get a single item from the app with the specified ID.
    /// </summary>
    /// <typeparam name="T">The type to convert to - usually inheriting `Custom.Data.CustomItem` or `CustomModel`</typeparam>
    /// <param name="id">the ID as an int</param>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="skipTypeCheck">allow get even if the Content-Type of the item with the ID doesn't match the type specified in the parameter T</param>
    /// <returns></returns>
    /// <remarks>
    /// Released in v17.03.
    /// </remarks>
    T? GetOne<T>(int id, NoParamOrder npo = default, bool skipTypeCheck = false)
        where T : class, IModelOfData;


    /// <summary>
    /// Get a single item from the app with the specified GUID.
    /// </summary>
    /// <typeparam name="T">The type to convert to - usually inheriting `Custom.Data.CustomItem` or `CustomModel`</typeparam>
    /// <param name="id">the ID as GUID</param>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="skipTypeCheck">allow get even if the Content-Type of the item with the ID doesn't match the type specified in the parameter T</param>
    /// <returns></returns>
    /// <remarks>
    /// Released in v17.03.
    /// </remarks>
    T? GetOne<T>(Guid id, NoParamOrder npo = default, bool skipTypeCheck = false)
        where T : class, IModelOfData;

    #endregion
}