using ToSic.Eav.Metadata;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Data.Sys.Typed;

namespace ToSic.Sxc.Apps.Sys.AppTyped;

// NOTE: not connecting Logs at the moment, and probably never needed
// as we're reworking it
// Remove the comment if it's finalized, otherwise if connecting is still needed, ensure it happens.
// #ToDoReviewConnectLogs


internal class AppDataTyped(Eav.DataSources.App.Dependencies services, Generator<AppDataCrud, AppDataCrud.Options> appDataCrud)
    : Eav.DataSources.App(services), IAppDataTyped
{
    #region Get Content Types - explicit implementation to ensure it's only available in Typed APIs

    IEnumerable<IContentType> IAppDataTyped.GetContentTypes()
        => AppReader.ContentTypes;

    IContentType? IAppDataTyped.GetContentType(string name)
        => AppReader.TryGetContentType(name);

    #endregion


    #region CRUD Data Operations

    private AppDataCrud Crud => field ??= appDataCrud.New(new(this, Reset));

    /// <inheritdoc />
    public IEntity Create(string contentTypeName, Dictionary<string, object?> values, string? userName = default, ITarget? target = default)
        => Crud.Create(contentTypeName, values, userName, target);

    /// <inheritdoc />
    public IEnumerable<IEntity> Create(string contentTypeName, IEnumerable<Dictionary<string, object?>> multiValues, string? userName = default)
        => Crud.Create(contentTypeName, multiValues, userName);

    /// <inheritdoc />
    public void Update(int entityId, Dictionary<string, object?> values, string? userName = default)
        => Crud.Update(entityId, values, userName);

    /// <inheritdoc />
    public void Delete(int entityId, string? userName = default)
        => Crud.Delete(entityId, userName);

    #endregion


    #region GetAll / GetOne

    /// <summary>
    /// Cdf Attachments and setup ToTypedHelper
    /// </summary>
    internal AppDataTyped Setup(ICodeDataFactory cdfConnected)
    {
        ToTypedHelper = new(cdfConnected, this, Log);
        return this;
    }

    [field: AllowNull, MaybeNull]
    private DataSourceToTypedHelper ToTypedHelper { get => field ?? throw new($"{nameof(ToTypedHelper)} not set"); set; }

    /// <inheritdoc />
    IEnumerable<T>? IAppDataTyped.GetAll<T>(NoParamOrder npo, string? typeName, bool nullIfNotFound)
        => ToTypedHelper.GetAllShared<T>(typeName, nullIfNotFound, false);

    /// <inheritdoc />
    T? IAppDataTyped.GetOne<T>(int id, NoParamOrder npo, bool skipTypeCheck)
        where T : class
        => ToTypedHelper.GetOne<T>(id, npo, skipTypeCheck);

    /// <inheritdoc />
    T? IAppDataTyped.GetOne<T>(Guid id, NoParamOrder npo, bool skipTypeCheck)
        where T : class
        => ToTypedHelper.GetOne<T>(id, npo, skipTypeCheck);

    #endregion

}
