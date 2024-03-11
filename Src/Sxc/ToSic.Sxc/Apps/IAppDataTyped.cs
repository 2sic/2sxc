using ToSic.Eav.DataSource;
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

    /// <inheritdoc cref="ToSic.Eav.Apps.IAppData.Create(string, Dictionary{string, object}, string, ITarget)"/>
    IEntity Create(string contentTypeName, Dictionary<string, object> values, string userName = null, ITarget target = null);

    /// <inheritdoc cref="ToSic.Eav.Apps.IAppData.Create(string, IEnumerable{Dictionary{string, object}}, string)"/>
    IEnumerable<IEntity> Create(string contentTypeName, IEnumerable<Dictionary<string, object>> multiValues, string userName = null);

    /// <inheritdoc cref="ToSic.Eav.Apps.IAppData.Update"/>
    void Update(int entityId, Dictionary<string, object> values, string userName = null);

    /// <inheritdoc cref="ToSic.Eav.Apps.IAppData.Delete"/>
    void Delete(int entityId, string userName = null);

    #endregion

    #region ContentTypes

    // Note: Implemented as a method, so later we can apply filters etc. as additional parameters
    /// <summary>
    /// All content types of the app.
    /// </summary>
    /// <remarks>Added v17</remarks>
    IEnumerable<IContentType> GetContentTypes();

    /// <summary>
    /// Get a single content type by name (display name or NameId).
    /// </summary>
    /// <param name="name">the name, either the normal name or the NameId which looks like a GUID</param>
    /// <remarks>Added v17</remarks>
    IContentType GetContentType(string name);

    #endregion

    #region GetAll, GetOne, GetMany WIP v17.02+

    /// <summary>
    /// Get all data from the app of the specified type.
    /// It will detect the expected Content-Type based on the name of the class used.
    /// So in most cases you will not add any parameters.
    /// </summary>
    /// <typeparam name="T">The type to get and convert to - usually inheriting <see cref="Custom.Data.CustomItem"/></typeparam>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="typeName">_optional_ type name</param>
    /// <param name="nullIfNotFound">if set, will return null if the type doesn't exist - default is empty list.</param>
    /// <returns></returns>
    /// <remarks>
    /// Released in v17.03.
    /// </remarks>
    public IEnumerable<T> GetAll<T>(NoParamOrder protector = default, string typeName = default,
        bool nullIfNotFound = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new();

    /// <summary>
    /// Get a single item from the app of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to get and convert to - usually inheriting <see cref="Custom.Data.CustomItem"/></typeparam>
    /// <param name="id">the ID as an int</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="skipTypeCheck">allow get even if the Content-Type of the item with the ID doesn't match the type specified in the parameter T</param>
    /// <returns></returns>
    /// <remarks>
    /// Released in v17.03.
    /// </remarks>
    T GetOne<T>(int id, NoParamOrder protector = default, bool skipTypeCheck = false)
        where T : class, ITypedItemWrapper16, ITypedItem, new();


    /// <summary>
    /// Get a single item from the app of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to get and convert to - usually inheriting <see cref="Custom.Data.CustomItem"/></typeparam>
    /// <param name="id">the ID as GUID</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="skipTypeCheck">allow get even if the Content-Type of the item with the ID doesn't match the type specified in the parameter T</param>
    /// <returns></returns>
    /// <remarks>
    /// Released in v17.03.
    /// </remarks>
    public T GetOne<T>(Guid id, NoParamOrder protector = default, bool skipTypeCheck = false)
        where T : class, ITypedItemWrapper16, ITypedItem, new();

    #endregion
}