using ToSic.Eav.WebApi.App;

namespace ToSic.Sxc.Backend.App;

/// <inheritdoc cref="IAppDataController" />
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppDataControllerReal(LazySvc<AppContent> appContentLazy)
    : ServiceBase("Api.DataRl", connect: [appContentLazy]), IAppDataController
{
    public const string LogSuffix = "Data";


    #region Get List / all of a certain content-type

    /// <inheritdoc />
    public IEnumerable<IDictionary<string, object>> GetEntities(string contentType, string appPath = default, string oDataSelect = default)
        => appContentLazy.Value.Init(appPath).GetItems(contentType, appPath, oDataSelect);

    #endregion


    #region GetOne by ID / GUID

    /// <inheritdoc />
    public IDictionary<string, object> GetOne(string contentType, string id, string appPath = default, string oDataSelect = default)
    {
        if(int.TryParse(id, out var intId))
            return GetAndSerializeOneAfterSecurityChecks(contentType,
                entityApi => entityApi.GetOrThrow(contentType, intId), appPath, oDataSelect);

        if (Guid.TryParse(id, out var guid))
            return GetAndSerializeOneAfterSecurityChecks(contentType,
                entityApi => entityApi.GetOrThrow(contentType, guid), appPath, oDataSelect);

#pragma warning disable S112 // General exceptions should never be thrown
        throw new("id neither int/guid, can't process");
#pragma warning restore S112 // General exceptions should never be thrown
    }

    /// <summary>
    /// Preprocess security / context, then get the item based on an passed in method,
    /// ...then process/finish
    /// </summary>
    /// <param name="contentType"></param>
    /// <param name="getOne"></param>
    /// <param name="appPath"></param>
    /// <returns></returns>
    private IDictionary<string, object> GetAndSerializeOneAfterSecurityChecks(string contentType, Func<IEnumerable<IEntity>, IEntity> getOne, string appPath, string oDataSelect)
    {
        return appContentLazy.Value.Init(appPath).GetOne(contentType, getOne, appPath, oDataSelect);
    }

    #endregion


    #region Create

    /// <inheritdoc />
    public IDictionary<string, object> CreateOrUpdate(string contentType, Dictionary<string, object> newContentItem, int? id = null,
        string appPath = null)
        => appContentLazy.Value.Init(appPath)
            .CreateOrUpdate(contentType, newContentItem, id, appPath);

    #endregion


    #region Delete

    /// <inheritdoc />
    public void Delete(string contentType, string id, string appPath = null)
    {
        if (int.TryParse(id, out var intId))
        {
            appContentLazy.Value.Init(appPath).Delete(contentType, intId, appPath);
            return;
        }

        if (Guid.TryParse(id, out var guid))
        {
            appContentLazy.Value.Init(appPath).Delete(contentType, guid, appPath);
            return;
        }

#pragma warning disable S112 // General exceptions should never be thrown
        throw new("id neither int/guid, can't process");
#pragma warning restore S112 // General exceptions should never be thrown
    }

    #endregion

}