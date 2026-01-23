using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Eav.Apps.Sys.Work;
using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Sys;
using ToSic.Eav.DataSource.Sys.Streams;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Security.Permissions;
using ToSic.Sys.Users;
using static ToSic.Eav.DataSource.DataSourceConstants;

namespace ToSic.Sxc.DataSources.Sys.Sources;

/// <inheritdoc />
/// <summary>
/// Keep only entities of a specific content-type
/// </summary>
[PrivateApi]
[VisualQuery(
    NiceName = "Entity-Picker (internal)",
    UiHint = "Special DataSource for the standard Entity-Picker",
    Icon = DataSourceIcons.RouteAlt,
    Type = DataSourceType.Filter,
    Audience = Audience.Advanced,
    NameId = "32369814-8f6d-47d8-a648-ce5372de78a8",
    DynamicOut = true,
    // ConfigurationType = "not yet defined", // ATM we don't expect a configuration
    HelpLink = "https://go.2sxc.org/todo")]

[ShowApiWhenReleased(ShowApiMode.Never)]
// ReSharper disable once UnusedMember.Global
public class EntityPicker : DataSourceBase
{
    #region Configuration-properties

    /// <summary>
    /// The name of the types to filter for.
    /// Either the normal name or the 'StaticName' which is usually a GUID.
    ///
    /// Can be many, comma separated
    /// </summary>
    [Configuration(Fallback = "[QueryString:TypeNames]")]
    public string? TypeNames => Configuration.GetThis();

    /// <summary>
    /// List of IDs to filter against - reducing the final set to just a few items
    /// </summary>
    [Configuration(Fallback = "[QueryString:ItemIds]")]
    public string? ItemIds => Configuration.GetThis();

    #endregion

    /// <inheritdoc />
    /// <summary>
    /// Constructs a new EntityTypeFilter
    /// </summary>
    [PrivateApi]
    public EntityPicker(
        GenWorkPlus<WorkEntities> workEntities,
        ICurrentContextService ctxService,
        Generator<MultiPermissionsApp> appPermissions,
        Generator<MultiPermissionsTypes> typePermissions,
        IUser user,
        IAppReaderFactory appReaders,
        Dependencies services
    ) : base(services, "Api.EntPck", connect: [workEntities, appPermissions, typePermissions, ctxService, appReaders])
    {
        _workEntities = workEntities;
        _ctxService = ctxService;
        _appPermissions = appPermissions;
        _typePermissions = typePermissions;
        _user = user;
        _appReaders = appReaders;
        ProvideOut(GetList);
    }

    private readonly GenWorkPlus<WorkEntities> _workEntities;
    private readonly ICurrentContextService _ctxService;
    private readonly IUser _user;
    private readonly IAppReaderFactory _appReaders;
    private readonly Generator<MultiPermissionsApp> _appPermissions;
    private readonly Generator<MultiPermissionsTypes> _typePermissions;

    #region Dynamic Out

    /// <summary>
    /// Override Out to provide the Default stream as well as additional streams for each content-type
    /// </summary>
    public override IReadOnlyDictionary<string, IDataStream> Out => _out.Get(() =>
    {
        // 0. If no names specified, then out is same as base out
        if (TypeNames.IsEmptyOrWs())
            return base.Out;
        var typesWithoutDefault = ContentTypes?
            .Where(ct => !ct.Name.EqualsInsensitive(StreamDefaultName))
            .ToList();
        if (typesWithoutDefault.SafeNone())
            return base.Out;

        // 1. Create a new StreamDictionary with the Default
        var outList = new StreamDictionary(this, Services.CacheService);
        outList.Add(StreamDefaultName, base.Out[StreamDefaultName]);

        // 2. Generate additional streams based on the content-types requested
        var list = base.Out[StreamDefaultName].List;
        foreach (var contentType in typesWithoutDefault)
        {
            var name = contentType.Name;
            var outStream = new DataStream(Services.CacheService, this, name, () => list.GetAll(contentType), true);
            outList.Add(name, outStream);
        }

        return outList.AsReadOnly();
    })!;

    private readonly GetOnce<IReadOnlyDictionary<string, IDataStream>> _out = new();

    #endregion

    private IEnumerable<IEntity> GetList()
    {
        // Open the log after config-parse, so we have type names
        var l = Log.Fn<IEnumerable<IEntity>>($"get list with type:{TypeNames}");

        // Get the context - must be pre-set by the caller

        var context = _ctxService.AppOrNull();
        if (context == null)
        {
            l.E("No App context");
            return l.Return(Error.Create(title: "No App context", message: "No context found, cannot continue"), "no context");
        }

        // Case 1: No Type Names - return all entities in the Content-Scope
        if (TypeNames.IsEmptyOrWs())
        {
            // App permission checker
            var permCheckApp = _appPermissions.New()
                .Init(context, this.PureIdentity());

            // First do security check with no-type name
            if (!permCheckApp.EnsureAll(GrantSets.ReadSomething, out _))
                return l.ReturnAsError(Error.Create(title: "No permissions on App, get all entities denied"));

            // Check if we provide drafts as well
            var withDrafts = permCheckApp.EnsureAny(GrantSets.ReadDraft);

            var entitiesSvc = _workEntities.New(this, showDrafts: withDrafts);
            var entities = entitiesSvc.OnlyContent(withConfiguration: _user.IsSystemAdmin).ToList();
            entities = FilterByIds(entities);
            return l.Return(entities, $"no type filter: {entities.Count}");
        }

        try
        {
            var types = ContentTypes;
            if (types == null! /* paranoid */)
                return l.ReturnAsError(Error.Create(title: "TypeList==null, something threw an error there."));
            if (!types.Any())
                return l.Return(new List<IEntity>(), "no valid types found, empty list");

            // Find all Entities of the specified types
            var result = new List<IEntity>();
            foreach (var type in types)
            {
                var lType = l.Fn($"Adding all of '{type.Name}'");

                var permCheckType = _typePermissions.New()
                    .Init(context, context.AppReaderRequired, type.Name);

                if (permCheckType.EnsureAll(GrantSets.ReadSomething, out _))
                {
                    var withDrafts = permCheckType.EnsureAny(GrantSets.ReadDraft);
                    var entitiesSvc = _workEntities.New(this, showDrafts: withDrafts);
                    var ofType = entitiesSvc.AppWorkCtx.Data.List.GetAll(type).ToList();
                    result.AddRange(ofType);
                    lType.Done($"{ofType.Count}");
                }
                else
                {
                    var errorItem = Error.Create(title: $"No permissions for Type {type.Name}");
                    result.AddRange(errorItem);
                    lType.Done($"added error for {type.Name}");
                }
            }

            if (result.Any())
                result = FilterByIds(result);

            return l.Return(result, $"typed/filtered: {result.Count}");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            /* ignore */
            return l.Return(Error.Create(title: "Something went wrong", message: "Unknown problem", exception: ex), "error");
        }

    }

    // TODO: CONTINUE
    // 1. Get the AppState from elsewhere, not the WorkEntities
    // 2. Get the ContentTypes to return both the types and a perm checker
    // 3. ...
    // CHANGE detection of type names to use the ContentTypes array ?

    /// <summary>
    /// List of ContentTypes to filter by
    /// </summary>
    [field: AllowNull, MaybeNull]
    private List<IContentType> ContentTypes => field ??= GetContentTypes();
    private List<IContentType> GetContentTypes()
    {
        var l = Log.Fn<List<IContentType>>();

        try
        {
            var typeNames = TypeNames
                .CsvToArrayWithoutEmpty()
                .ToList();

            l.A($"found {typeNames.Count} type names, before verifying if they exist");

            if (!typeNames.Any())
                return l.Return([]);

            var appReader = _appReaders.Get(this.PureIdentity());

            var types = typeNames
                .Select(appReader.TryGetContentType)
                .Where(t => t != null)
                .Cast<IContentType>()
                .ToList();

            return l.Return(types, $"found {types.Count}");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            /* ignore */
            return l.ReturnAsError([]);
        }
    }

    private List<IEntity> FilterByIds(List<IEntity> list)
    {
        var l = Log.Fn<List<IEntity>>($"started with {list.Count}");
        var rawIds = ItemIds;
        if (rawIds.IsEmptyOrWs())
            return l.Return(list, "no filter, return all");

        var untyped = rawIds.CsvToArrayWithoutEmpty().ToList();

        if (!untyped.Any())
            return l.Return(list, "empty filter, return all");

        var result = new List<IEntity>();
        foreach (var id in untyped)
        {
            IEntity? found = null;
            // check if id is int or guid
            if (Guid.TryParse(id, out var guid))
                found = list.GetOne(guid);
            else if (int.TryParse(id, out var intId))
                found = list.GetOne(intId);
            if (found != null)
                result.Add(found);
        }
        return l.Return(result, $"filtered to {result.Count}");
    }

}