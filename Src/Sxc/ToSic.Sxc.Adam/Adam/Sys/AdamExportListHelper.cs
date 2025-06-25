using ToSic.Eav.Apps.Assets.Internal;
using ToSic.Sxc.Adam.Manager.Internal;
using ToSic.Sxc.Adam.Sys.FileSystem;

namespace ToSic.Sxc.Adam.Sys;

/// <summary>
/// Export helper
/// provides a list of all files / folders in ADAM for export
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamExportListHelper<TFolderId, TFileId>(AdamManager adm)
{
    private readonly IFolder _root = adm.RootFolder ?? throw new ArgumentNullException(nameof(adm.RootFolder), @"needs a root folder to export from ADAM");
    // todo #adamid - should use TFile/TFolder
    private readonly List<TFileId> _fileIds = [];
    private readonly List<TFolderId> _folderIds = [];

    private readonly IAdamFileSystem _envFs = adm.AdamFs;

    public List<TFileId> AppFiles
    {
        get
        {
            if (_fileIds.Count == 0)
                AddFolder(_root);
            return _fileIds;
        }
    }

    public List<TFolderId> AppFolders
    {
        get
        {
            if (_folderIds.Count == 0)
                AddFolder(_root);
            return _folderIds;
        }
            
    } 
    private void AddFolder(IFolder folder)
    {
        _folderIds.Add(((IAssetSysId<TFolderId>)folder).SysId);  // track of the folder
        AddFilesInFolder(folder);   // keep track of the files

        foreach (var f in _envFs.GetFolders(folder))   // then add subfolders
            AddFolder(f);
    }

    private void AddFilesInFolder(IFolder folder) 
        => _envFs.GetFiles(folder).ForEach(f => _fileIds.Add(((IAssetSysId<TFileId>)f).SysId));
}