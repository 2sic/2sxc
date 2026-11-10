using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Apps.Sys;

// NOTE: not connecting Logs at the moment, and probably never needed
// as we're reworking it
// Remove the comment if it's finalized, otherwise if connecting is still needed, ensure it happens.
// #ToDoReviewConnectLogs


/// <summary>
/// The Data object on an App. It's also a data-source of type <see cref="Eav.DataSources.App"/>,
/// so it has many streams, one for each content-type so you can use it in combination with other DataSources. <br/>
/// The special feature is that it also has methods for data-manipulation,
/// including Create, Update and Delete
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class AppDataDynamic(Eav.DataSources.App.Dependencies services, Generator<AppDataCrud, AppDataCrud.Options> appDataCrud)
    : Eav.DataSources.App(services), IAppData
{

    #region Get Content Type (added in v20 to better fix Mobius compatibility)

    IContentType? IAppData.GetContentType(string name)
        => AppReader.TryGetContentType(name);

    IEnumerable<IContentType> IAppData.GetContentTypes()
        => AppReader.ContentTypes;

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

    #region Get Metadata

    /// <inheritdoc />
    public IEnumerable<IEntity> GetMetadata<TKey>(int targetType, TKey key, string? contentTypeName = null)
        => AppReader.Metadata.GetMetadata(targetType, key, contentTypeName);

    /// <inheritdoc />
    public IEnumerable<IEntity> GetMetadata<TKey>(TargetTypes targetType, TKey key, string? contentTypeName = null)
        => AppReader.Metadata.GetMetadata(targetType, key, contentTypeName);

    /// <inheritdoc />
    public IEnumerable<IEntity> GetCustomMetadata<TKey>(TKey key, string? contentTypeName = null)
        => AppReader.Metadata.GetMetadata(TargetTypes.Custom, key, contentTypeName);

    #endregion

}