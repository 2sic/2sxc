using ToSic.Eav.Data.Sys;
using ToSic.Eav.DataSource.Sys.Caching;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Apps.Sys.Api01;

namespace ToSic.Sxc.Apps.Sys;

/// <summary>
/// The Data object on an App. It's also a data-source of type <see cref="Eav.DataSources.App"/>,
/// so it has many streams, one for each content-type so you can use it in combination with other DataSources. <br/>
/// The special feature is that it also has methods for data-manipulation,
/// including Create, Update and Delete
/// </summary>
[PrivateApi("May have been visible before v17, but probably not really, name was also different. Published through the IAppData interface.")]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class AppDataWithCrud : Eav.DataSources.App, IAppData
{
    private readonly LazySvc<IDataSourceCacheService> _dsCacheSvc;

    #region Constructor stuff

    public AppDataWithCrud(Dependencies services, LazySvc<SimpleDataEditService> dataController, LazySvc<IDataSourceCacheService> dsCacheSvc) : base(services)
    {
        ConnectLogs([
            DataController = dataController.SetInit(dc => dc.Init(ZoneId, AppId, false)),
            _dsCacheSvc = dsCacheSvc
        ]);
    }

    #endregion

    #region Get Content Type (added in v20 to better fix Mobius compatbility)

    IContentType? IAppData.GetContentType(string name)
        => AppReader.TryGetContentType(name);

    IEnumerable<IContentType> IAppData.GetContentTypes()
        => AppReader.ContentTypes;

    #endregion

    #region CRUD Data Operations

    /// <summary>
    /// Get a correctly instantiated instance of the simple data controller once needed.
    /// </summary>
    /// <returns>A data controller to create, update and delete entities</returns>
    private LazySvc<SimpleDataEditService> DataController { get; }

    /// <inheritdoc />
    public IEntity Create(string contentTypeName, Dictionary<string, object?> values, string? userName = default, ITarget? target = default)
    {
        var l = Log.Fn<IEntity>(contentTypeName);
        // Ensure case insensitive
        values = values.ToInvariant();

        if (!string.IsNullOrEmpty(userName))
            ProvideOwnerInValues(values, userName!); // userName should be in 2sxc user IdentityToken format (like 'dnn:user=N')
        var ids = DataController.Value.Create(contentTypeName, new List<Dictionary<string, object?>> { values! }, target);
        var id = ids.FirstOrDefault();
        FlushDataSnapshot();
        // try to find it again (AppState.List contains also draft items)
        var created = AppReader.List.GetOne(id)
            ?? throw new ArgumentException($"Can't find the freshly created entity with ID {id}, something is wrong.");
        return l.Return(created, $"{created.EntityId}/{created.EntityGuid}");
    }

    private static void ProvideOwnerInValues(IDictionary<string, object?> values, string userIdentityToken)
    {
        // userIdentityToken is not simple 'userName' string, but 2sxc user IdentityToken structure (like 'dnn:user=N')
        if (values.ContainsKey(AttributeNames.EntityFieldOwner))
            return;
        values.Add(AttributeNames.EntityFieldOwner, userIdentityToken);
    }

    /// <inheritdoc />
    public IEnumerable<IEntity> Create(string contentTypeName, IEnumerable<Dictionary<string, object?>> multiValues, string? userName = default)
    {
        var valueList = multiValues.ToListOpt();
        var l = Log.Fn<IEnumerable<IEntity>>($"app create many ({valueList.Count}) new entities of type:{contentTypeName}");
        // ensure case insensitive
        multiValues = valueList
            .Select(mv => mv.ToInvariant())
            .ToList();

        if (!string.IsNullOrEmpty(userName))
            foreach (var values in multiValues)
                ProvideOwnerInValues(values, userName!); // userName should be in 2sxc user IdentityToken format (like 'dnn:user=N')

        var ids = DataController.Value.Create(contentTypeName, multiValues);
        FlushDataSnapshot();
        var created = List
            .Where(e => ids.Contains(e.EntityId))
            .ToList();
        return l.Return(created, $"{created.Count}");
    }

    /// <inheritdoc />
    public void Update(int entityId, Dictionary<string, object?> values, string? userName = default)
    {
        var l = Log.Fn($"app update i:{entityId}");
        // FYI: userName is not used (to change owner of updated entity).
        DataController.Value.Update(entityId, values);
        FlushDataSnapshot();
        l.Done();
    }


    /// <inheritdoc />
    public void Delete(int entityId, string? userName = default)
    {
        var l = Log.Fn($"app delete i:{entityId}");
        // FYI: userName is not used (to change owner of deleted entity).
        DataController.Value.Delete(entityId);
        FlushDataSnapshot();
        l.Done();
    }

    /// <summary>
    /// All 2sxc data is always snapshot, so read will only run a query once and keep it till the objects are killed.
    /// If we do updates or perform other changes, we must clear the current snapshot so subsequent access will result
    /// in the new data. 
    /// </summary>
    private void FlushDataSnapshot()
    {
        // Purge the list and parent lists - must happen first, as otherwise the list-access will be interrupted
        _dsCacheSvc.Value.Flush(this, true);
        Reset();
    }

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