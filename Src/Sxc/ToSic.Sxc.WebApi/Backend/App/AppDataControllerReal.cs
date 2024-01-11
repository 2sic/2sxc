using ToSic.Eav.WebApi.App;

namespace ToSic.Sxc.Backend.App;

/// <inheritdoc />
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppDataControllerReal: ServiceBase, IAppDataController
{
    public const string LogSuffix = "Data";

    public AppDataControllerReal(LazySvc<AppContent> appContentLazy): base("Api.DataRl")
    {
        ConnectServices(
            _appContentLazy = appContentLazy
        );
    }
    private readonly LazySvc<AppContent> _appContentLazy;


    #region Get List / all of a certain content-type

    /// <inheritdoc />
    public IEnumerable<IDictionary<string, object>> GetEntities(string contentType, string appPath = null)
        => _appContentLazy.Value.Init(appPath).GetItems(contentType, appPath);

    #endregion


    #region GetOne by ID / GUID

    /// <inheritdoc />
    public IDictionary<string, object> GetOne(string contentType, string id, string appPath = null)
    {
        if(int.TryParse(id, out var intId))
            return GetAndSerializeOneAfterSecurityChecks(contentType,
                entityApi => entityApi.GetOrThrow(contentType, intId), appPath);

        if (Guid.TryParse(id, out var guid))
            return GetAndSerializeOneAfterSecurityChecks(contentType,
                entityApi => entityApi.GetOrThrow(contentType, guid), appPath);

#pragma warning disable S112 // General exceptions should never be thrown
        throw new Exception("id neither int/guid, can't process");
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
    private IDictionary<string, object> GetAndSerializeOneAfterSecurityChecks(string contentType, Func<IEnumerable<IEntity>, IEntity> getOne, string appPath)
        => _appContentLazy.Value.Init(appPath).GetOne(contentType, getOne, appPath);

    #endregion


    #region Create

    /// <inheritdoc />
    public IDictionary<string, object> CreateOrUpdate(string contentType, Dictionary<string, object> newContentItem, int? id = null,
        string appPath = null)
        => _appContentLazy.Value.Init(appPath)
            .CreateOrUpdate(contentType, newContentItem, id, appPath);

    #endregion


    #region Delete

    /// <inheritdoc />
    public void Delete(string contentType, string id, string appPath = null)
    {
        if (int.TryParse(id, out var intId))
        {
            _appContentLazy.Value.Init(appPath).Delete(contentType, intId, appPath);
            return;
        }

        if (Guid.TryParse(id, out var guid))
        {
            _appContentLazy.Value.Init(appPath).Delete(contentType, guid, appPath);
            return;
        }

#pragma warning disable S112 // General exceptions should never be thrown
        throw new Exception("id neither int/guid, can't process");
#pragma warning restore S112 // General exceptions should never be thrown
    }

    #endregion

}