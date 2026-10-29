using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.WebApi.Sys.App;

namespace ToSic.Sxc.Backend.App;

/// <inheritdoc cref="IAppDataController" />
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppDataControllerReal(Generator<AppContent, AppContent.Options> appContentGen)
    : ServiceBase("Api.DataRl", connect: [appContentGen])/*, IAppDataController*/
{
    public const string LogSuffix = "Data";


    #region Get List / all of a certain content-type

    /// <inheritdoc />
    public IEnumerable<IDictionary<string, object>> GetEntities(string contentType, string? appPath = default, Uri? uri = null)
        => appContentGen.New(new(appPath)).GetItems(contentType, appPath, uri);

    #endregion


    #region GetOne by ID / GUID

    /// <inheritdoc />
    public IDictionary<string, object> GetOne(string contentType, string id, string? appPath = default, Uri? uri = null)
    {
        if(int.TryParse(id, out var intId))
            return GetAndSerializeOneAfterSecurityChecks(contentType,
                entityApi => entityApi.GetOrThrow(contentType, intId), appPath, uri);

        if (Guid.TryParse(id, out var guid))
            return GetAndSerializeOneAfterSecurityChecks(contentType,
                entityApi => entityApi.GetOrThrow(contentType, guid), appPath, uri);

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
    /// <param name="oDataSelect"></param>
    /// <param name="uri"></param>
    /// <returns></returns>
    private IDictionary<string, object> GetAndSerializeOneAfterSecurityChecks(string contentType, Func<IEnumerable<IEntity>, IEntity> getOne, string? appPath, Uri? uri)
        => appContentGen.New(new(appPath)).GetOne(contentType, getOne, appPath, uri);

    #endregion


    #region Create

    /// <inheritdoc />
    public IDictionary<string, object> CreateOrUpdate(string contentType, Dictionary<string, object?> newContentItem, int? id = null,
        string? appPath = null)
        => appContentGen.New(new(appPath))
            .CreateOrUpdate(contentType, newContentItem, id, appPath);

    #endregion


    #region Delete

    /// <inheritdoc />
    public void Delete(string contentType, string id, string? appPath = null)
    {
        if (int.TryParse(id, out var intId))
        {
            appContentGen.New(new(appPath)).Delete(contentType, intId, appPath);
            return;
        }

        if (Guid.TryParse(id, out var guid))
        {
            appContentGen.New(new(appPath)).Delete(contentType, guid, appPath);
            return;
        }

#pragma warning disable S112 // General exceptions should never be thrown
        throw new("id neither int/guid, can't process");
#pragma warning restore S112 // General exceptions should never be thrown
    }

    #endregion

}