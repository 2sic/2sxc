using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Data;

/// <summary>
/// Query object from <see cref="IAppTyped.GetQuery"/> an App in **Typed** mode, with quick get-and-convert-helpers such as `GetAll` or `GetOne`.
/// </summary>
/// <remarks>
/// WIP v20
/// </remarks>
[WorkInProgressApi("Still WIP")]
public interface ITypedQuery: IDataSource
{
    #region GetAll, GetOne, GetMany WIP v17.02+

    /// <summary>
    /// Get all data from the app of the specified type in a stream with the same name (or "Default").
    /// It will detect the expected stream name based on the name of the class used.
    /// 
    /// So in most cases you will not add any parameters except for the type parameter `T`.
    /// This is usually a type of your `AppCode.Data` namespace.
    /// </summary>
    /// <typeparam name="T">The type to convert to - usually inheriting `Custom.Data.CustomItem` or `CustomModel`</typeparam>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="typeName">_optional_ type name which is used as the **stream** name when retrieving the data, as each stream contains entities of one type.</param>
    /// <param name="nullIfNotFound">if set, will return null if the type doesn't exist - default is empty list.</param>
    /// <returns></returns>
    /// <remarks>
    /// WIP in v20.00
    /// </remarks>
    IEnumerable<T>? GetAll<T>(NoParamOrder npo = default, string? typeName = default, bool nullIfNotFound = default)
        where T : class, ICanWrapData;

    /// <summary>
    /// Get a single item from the query with the specified ID.
    /// </summary>
    /// <typeparam name="T">The type to convert to - usually inheriting `Custom.Data.CustomItem` or `CustomModel`</typeparam>
    /// <param name="id">the ID as an int</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="skipTypeCheck">allow get even if the Content-Type of the item with the ID doesn't match the type specified in the parameter T</param>
    /// <returns></returns>
    /// <remarks>
    /// WIP in v20.00
    /// </remarks>
    T? GetOne<T>(int id, NoParamOrder npo = default, bool skipTypeCheck = false)
        where T : class, ICanWrapData;


    /// <summary>
    /// Get a single item from the query with the specified GUID.
    /// </summary>
    /// <typeparam name="T">The type to convert to - usually inheriting `Custom.Data.CustomItem` or `CustomModel`</typeparam>
    /// <param name="guid">the ID as GUID</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="skipTypeCheck">allow get even if the Content-Type of the item with the ID doesn't match the type specified in the parameter T</param>
    /// <returns></returns>
    /// <remarks>
    /// WIP in v20.00
    /// </remarks>
    T? GetOne<T>(Guid guid, NoParamOrder npo = default, bool skipTypeCheck = false)
        where T : class, ICanWrapData;

    #endregion
}