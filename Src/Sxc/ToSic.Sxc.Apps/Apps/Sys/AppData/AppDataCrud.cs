using ToSic.Eav.Data.Sys;
using ToSic.Eav.DataSource.Sys.Caching;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Apps.Sys.Api01;

namespace ToSic.Sxc.Apps.Sys;

/// <summary>
/// The CRUD (Create, Read, Update, Delete) helper for the Sxc App objects in Razor/WebApis.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppDataCrud(IAppReaderFactory appReaders, Generator<SimpleDataEditService, SimpleDataEditService.Options> dataEditSvcGen, IDataSourceCacheService dsCacheSvc)
    : ServiceWithSetup<AppDataCrud.Options>("Sxc.ApCrud", connect: [appReaders, dataEditSvcGen, dsCacheSvc])
{
    /// <summary>
    /// Options to start the Crud helper.
    /// </summary>
    /// <param name="AppDataSource">The dynamic/typed AppData implementation.</param>
    /// <param name="ResetCallback">Reset callback to re-populate the lists.</param>
    public record Options(IDataSource AppDataSource, Action ResetCallback);

    private IAppReader AppReader => field ??= appReaders.Get(MyOptions.AppDataSource);

    /// <summary>
    /// Get a correctly instantiated instance of the simple data controller once needed.
    /// </summary>
    private SimpleDataEditService DataEditSvc => field ??= dataEditSvcGen.New(new(MyOptions.AppDataSource.ZoneId, MyOptions.AppDataSource.AppId, false));

    public IEntity Create(string contentTypeName, Dictionary<string, object?> values, string? userName = default, ITarget? target = default)
    {
        var l = Log.Fn<IEntity>(contentTypeName);
        // Ensure case insensitive
        values = values.ToInvariant();

        // Add owner if not already set
        // the userName / userIdentityToken is a 2sxc user IdentityToken structure (like 'dnn:user=N')
        if (!string.IsNullOrEmpty(userName))
            if (!values.ContainsKey(AttributeNames.EntityFieldOwner))
                values.Add(AttributeNames.EntityFieldOwner, userName);
        var ids = DataEditSvc.Create(contentTypeName, new List<Dictionary<string, object?>> { values }, target);
        var id = ids.FirstOrDefault();
        FlushDataSnapshot();
        // try to find it again (AppState.List contains also draft items)
        var created = AppReader.List.GetOne(id)
                      ?? throw new ArgumentException($"Can't find the freshly created entity with ID {id}, something is wrong.");
        return l.Return(created, $"{created.EntityId}/{created.EntityGuid}");
    }


    public IEnumerable<IEntity> Create(string contentTypeName, IEnumerable<Dictionary<string, object?>> multiValues, string? userName = default)
    {
        var valueList = multiValues.ToListOpt();
        var l = Log.Fn<IEnumerable<IEntity>>($"app create many ({valueList.Count}) new entities of type:{contentTypeName}");
        // ensure case insensitive
        var invariant = valueList.Select(mv => mv.ToInvariant()).ToList();

        // Add owner if not already set
        // the userName / userIdentityToken is a 2sxc user IdentityToken structure (like 'dnn:user=N')
        if (!string.IsNullOrEmpty(userName))
            foreach (var values in invariant)
                if (!values.ContainsKey(AttributeNames.EntityFieldOwner))
                    values.Add(AttributeNames.EntityFieldOwner, userName);

        var ids = DataEditSvc.Create(contentTypeName, invariant);
        FlushDataSnapshot();
        var created = MyOptions.AppDataSource.List
            .Where(e => ids.Contains(e.EntityId))
            .ToList();
        return l.Return(created, $"{created.Count}");
    }


    public void Update(int entityId, Dictionary<string, object?> values, string? userName = default)
    {
        var l = Log.Fn($"app update i:{entityId}");
        // FYI: userName is not used (to change owner of updated entity).
        DataEditSvc.Update(entityId, values);
        FlushDataSnapshot();
        l.Done();
    }


    public void Delete(int entityId, string? userName = default)
    {
        var l = Log.Fn($"app delete i:{entityId}");
        // FYI: userName is not used (to change owner of deleted entity).
        DataEditSvc.Delete(entityId);
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
        dsCacheSvc.Flush(MyOptions.AppDataSource, true);
        MyOptions.ResetCallback();
    }

}
