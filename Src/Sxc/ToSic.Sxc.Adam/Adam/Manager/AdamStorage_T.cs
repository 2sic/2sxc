﻿using ToSic.Lib.Helpers;

namespace ToSic.Sxc.Adam.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class AdamStorage<TFolderId, TFileId>() : AdamStorage("Adm.Base")
{
    public void Init(AdamManager<TFolderId, TFileId> manager) => Manager = manager;

    protected AdamManager<TFolderId, TFileId> Manager { get; private set; }


    /// <summary>
    /// Get the folder specified in App.Settings (BasePath) combined with the module's ID
    /// </summary>
    /// <remarks>
    /// Will create the folder if it does not exist
    /// </remarks>
    public Folder<TFolderId, TFileId> Folder(string subFolder, bool autoCreate)
    {
        var l = Log.Fn<Folder<TFolderId, TFileId>>($"{nameof(Folder)}(\"{subFolder}\", {autoCreate})");
        var fld = Manager.Folder(GeneratePath(subFolder), autoCreate);
        return l.ReturnAsOk(fld);
    }


    /// <summary>
    /// Get a (root) folder object for this container
    /// </summary>
    /// <returns></returns>
    public Folder<TFolderId, TFileId> Folder(bool autoCreate = false) => _folder.Get(() => Folder("", autoCreate));
    private readonly GetOnce<Folder<TFolderId, TFileId>> _folder = new();

}