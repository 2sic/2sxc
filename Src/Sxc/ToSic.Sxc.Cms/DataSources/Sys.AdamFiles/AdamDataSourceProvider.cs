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
public class AdamDataSourceProvider<TFolderId, TFileId>(Generator<AdamContext, AdamContext.Options> adamContext, IContextOfAppFactory appCtxFactory)
    : ServiceBase($"{SxcLogName}.AdamDs", connect: [adamContext, appCtxFactory])
{
    public Func<IEntity, IEnumerable<AdamItemDataRaw>> GetInternal(int appId)
    {
        var context = appCtxFactory.Create(appId);
        return entity => GetAdamListOfItems(context, entity);
    }

    private IEnumerable<AdamItemDataRaw> GetAdamListOfItems(IContextOfApp context, IEntity entity)
    {
        var l = Log.Fn<IEnumerable<AdamItemDataRaw>>();

        // TODO: this is just tmp code to get some data...
        var adamCtx = adamContext.New(new(context, entity.Type.Name, string.Empty, entity.EntityGuid, false));

        // get root and at the same time auto-create the core folder in case it's missing (important)
        var root = adamCtx.AdamRoot.RootFolder(false);

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