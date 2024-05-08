using ToSic.Eav.Context;
using ToSic.Eav.Metadata;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.DataSources.Internal;

/// <summary>
/// Base class to provide data to the Adam DataSource.
///
/// Must be overriden in each platform.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AdamDataSourceProvider<TFolderId, TFileId> : ServiceBase<AdamDataSourceProvider<TFolderId, TFileId>.MyServices>
{
    private readonly MyServices _services;
    private IContextOfApp _context;

    public class MyServices : MyServicesBase
    {
        public LazySvc<AdamContext<TFolderId, TFileId>> AdamContext { get; }
        public ISxcContextResolver CtxResolver { get; }

        /// <summary>
        /// Note that we will use Generators for safety, because in rare cases the dependencies could be re-used to create a sub-data-source
        /// </summary>
        public MyServices(
            LazySvc<AdamContext<TFolderId, TFileId>> adamContext,
            ISxcContextResolver ctxResolver
        )
        {
            ConnectLogs([
                AdamContext = adamContext,
                CtxResolver = ctxResolver
            ]);
        }
    }

    protected AdamDataSourceProvider(MyServices services) : base(services, $"{SxcLogName}.AdamDs")
    {
        _services = services;
    }

    public AdamDataSourceProvider<TFolderId, TFileId> Configure(
        NoParamOrder noParamOrder = default,
        int appId = default,
        string entityIds = default,
        string entityGuids = default,
        string fields = default,
        string filter = default
    ) => Log.Func($"a:{appId}; entityIds:{entityIds}, entityGuids:{entityGuids}, fields:{fields}, filter:{filter}", l =>
    {
        _context = appId > 0 ? Services.CtxResolver.GetBlockOrSetApp(appId) : Services.CtxResolver.AppNameRouteBlock(null);
        _entityIds = entityIds;
        _entityGuids = entityGuids;
        _fields = fields;
        _filter = filter;
        return this;
    });

    private string _entityIds;
    private string _entityGuids;
    private string _fields;
    private string _filter;


    public Func<IEntity, IEnumerable<AdamItemDataRaw>> GetInternal()
        => GetAdamListOfItems;

    private IEnumerable<AdamItemDataRaw> GetAdamListOfItems(IEntity entity) => Log.Func(() =>
    {
        // This will contain the list of items
        var list = new List<AdamItemDataRaw>();

        // TODO: this is just tmp code to get some data...
        _services.AdamContext.Value.Init(_context, entity.Type.Name, string.Empty, entity.EntityGuid, false, cdf: null);

        // get root and at the same time auto-create the core folder in case it's missing (important)
        var root = _services.AdamContext.Value.AdamRoot.Folder(false);

        // if no root exists then quit now
        if (root == null)
            return ([], "null/empty");

        AddAdamItemsFromFolder(root, list);

        return (list, $"found:{list.Count}");
    });

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