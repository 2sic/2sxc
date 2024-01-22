namespace ToSic.Sxc.Adam.Internal;

/// <summary>
/// Export helper
/// provides a list of all files / folders in ADAM for export
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class Export<TFolderId, TFileId>(AdamManager<TFolderId, TFileId> adm)
{
    private readonly Folder<TFolderId, TFileId> _root = adm.RootFolder;
    // todo #adamid - should use TFile/TFolder
    private readonly List<TFileId> _fileIds = [];
    private readonly List<TFolderId> _folderIds = [];

    private readonly IAdamFileSystem<TFolderId, TFileId> _envFs = adm.AdamFs;

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
    private void AddFolder(Folder<TFolderId, TFileId> folder)
    {
        _folderIds.Add(folder.SysId);  // track of the folder
        AddFilesInFolder(folder);   // keep track of the files

        foreach (var f in _envFs.GetFolders(folder))   // then add subfolders
            AddFolder(f);
    }

    private void AddFilesInFolder(IFolder folder) 
        => _envFs.GetFiles(folder).ForEach(f => _fileIds.Add(f.SysId));
}