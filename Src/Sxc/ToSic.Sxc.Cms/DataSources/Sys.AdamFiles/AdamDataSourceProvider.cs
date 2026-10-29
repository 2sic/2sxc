using ToSic.Eav.Context;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Sys.Manager;

namespace ToSic.Sxc.DataSources;

/// <summary>
/// Base class to provide data to the Adam DataSource.
///
/// Must be overriden in each platform.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamDataSourceProvider<TFolderId, TFileId>(AdamDataSourceProvider<TFolderId, TFileId>.Dependencies services)
    : ServiceBase<AdamDataSourceProvider<TFolderId, TFileId>.Dependencies>(services, $"{SxcLogName}.AdamDs")
{
    private IContextOfApp _context = null!;

    public record Dependencies(LazySvc<AdamContext> AdamContext, IContextOfAppFactory AppCtxFactory)
        : DependenciesBase(connect: [AdamContext, AppCtxFactory]);

    public AdamDataSourceProvider<TFolderId, TFileId> Configure(NoParamOrder npo = default, int appId = default)
    {
        var l = Log.Fn<AdamDataSourceProvider<TFolderId, TFileId>>($"a:{appId}");

        // 2026-10-20 2dm: Believe that the AppId should always have a valid value, so no magic find-it-yourself
        // Turned this off for now, keep till early 2027 then clean up.
        // If something does explode, try to ensure that the calling path sets a proper AppId _before_ calling this
        //_context = appId > 0
        //    ? Services.CtxService.GetExistingAppOrSet(appId)
        //    : Services.CtxService.AppNameRouteBlock(null);

        _context = Services.AppCtxFactory.Create(appId);
        return l.Return(this);
    }


    public Func<IEntity, IEnumerable<AdamItemDataRaw>> GetInternal()
        => GetAdamListOfItems;

    private IEnumerable<AdamItemDataRaw> GetAdamListOfItems(IEntity entity)
    {
        var l = Log.Fn<IEnumerable<AdamItemDataRaw>>();

        // TODO: this is just tmp code to get some data...
        Services.AdamContext.Value
            .Init(_context, entity.Type.Name, string.Empty, entity.EntityGuid, false);

        // get root and at the same time auto-create the core folder in case it's missing (important)
        var root = Services.AdamContext.Value.AdamRoot.RootFolder(false);

        // if no root exists then quit now
        if (root == null)
            return l.Return([], "null/empty");

        // This will contain the list of items
        var list = new List<AdamItemDataRaw>();

        // Call add recursively to add all items from the root folder and its subfolders
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