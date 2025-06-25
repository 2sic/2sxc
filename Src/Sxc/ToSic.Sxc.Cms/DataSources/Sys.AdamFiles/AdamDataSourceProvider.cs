using ToSic.Eav.Context;
using ToSic.Eav.Metadata;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.DataSources;

/// <summary>
/// Base class to provide data to the Adam DataSource.
///
/// Must be overriden in each platform.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamDataSourceProvider<TFolderId, TFileId> : ServiceBase<AdamDataSourceProvider<TFolderId, TFileId>.MyServices>
{
    private IContextOfApp _context = null!;

    public class MyServices(LazySvc<AdamContext> adamContext, ISxcAppCurrentContextService ctxService)
        : MyServicesBase(connect: [adamContext, ctxService])
    {
        public LazySvc<AdamContext> AdamContext { get; } = adamContext;
        public ISxcAppCurrentContextService CtxService { get; } = ctxService;
    }

    protected AdamDataSourceProvider(MyServices services) : base(services, $"{SxcLogName}.AdamDs")
    { }

    public AdamDataSourceProvider<TFolderId, TFileId> Configure(
        NoParamOrder noParamOrder = default,
        int appId = default,
        string? entityIds = default,
        string? entityGuids = default,
        string? fields = default,
        string? filter = default
    )
    {
        var l = Log.Fn<AdamDataSourceProvider<TFolderId, TFileId>>($"a:{appId}; entityIds:{entityIds}, entityGuids:{entityGuids}, fields:{fields}, filter:{filter}");
        _context = appId > 0
            ? Services.CtxService.GetExistingAppOrSet(appId)
            : Services.CtxService.AppNameRouteBlock(null);
        _entityIds = entityIds;
        _entityGuids = entityGuids;
        _fields = fields;
        _filter = filter;
        return l.Return(this);
    }

    private string? _entityIds;
    private string? _entityGuids;
    private string? _fields;
    private string? _filter;


    public Func<IEntity, IEnumerable<AdamItemDataRaw>> GetInternal()
        => GetAdamListOfItems;

    private IEnumerable<AdamItemDataRaw> GetAdamListOfItems(IEntity entity)
    {
        var l = Log.Fn<IEnumerable<AdamItemDataRaw>>();
        // This will contain the list of items
        var list = new List<AdamItemDataRaw>();

        // TODO: this is just tmp code to get some data...
        Services.AdamContext.Value
            .Init(_context, entity.Type.Name, string.Empty, entity.EntityGuid, false);

        // get root and at the same time auto-create the core folder in case it's missing (important)
        var root = Services.AdamContext.Value.AdamRoot.RootFolder(false);

        // if no root exists then quit now
        if (root == null)
            return l.Return([], "null/empty");

        AddAdamItemsFromFolder(root, list);

        return l.Return(list, $"found:{list.Count}");
    }

    private void AddAdamItemsFromFolder(IFolder folder, List<AdamItemDataRaw> list)
    {
        list.AddRange(folder.Folders.Select(f => new AdamItemDataRaw
        {
            Name = f.Name,
            ReferenceId = (f as IHasMetadata).Metadata.Target.KeyString,
            Url = f.Url,
            Type = f.Type,
            IsFolder = true,
            Size = 0,
            Path = f.Path,
            Created = f.Created,
            Modified = f.Modified
        }));
        list.AddRange(folder.Files.Select(f => new AdamItemDataRaw
        {
            Name = f.Name,
            ReferenceId = (f as IHasMetadata).Metadata.Target.KeyString,
            Url = f.Url,
            Type = f.Type,
            IsFolder = false,
            Size = f.Size,
            Path = f.Path,
            Created = f.Created,
            Modified = f.Modified
        }));
        foreach (var subFolder in folder.Folders) 
            AddAdamItemsFromFolder(subFolder, list);
    }
}