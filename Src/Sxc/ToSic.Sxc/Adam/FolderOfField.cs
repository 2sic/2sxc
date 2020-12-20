using System;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// The ADAM Navigator creates a folder object for an entity/field combination
    /// This is the root folder where all files for this field are stored
    /// </summary>
    public class FolderOfField<TFolderId, TFileId> : Folder<TFolderId, TFileId>
    {
        public FolderOfField(AdamManager<TFolderId, TFileId> adamManager, Guid entityGuid, string fieldName): base(adamManager)
        {
            var adamOfField = new AdamOfField<TFolderId, TFileId>(AdamManager, entityGuid, fieldName);

            if (!AdamManager.Exists(adamOfField.Root)) return;

            var f = AdamManager.Folder(adamOfField.Root);
            if (f == null) return;

            Path = f.Path;
            Modified = f.Modified;
            SysId = f.SysId;
            Created = f.Created;
            Modified = f.Modified;

            // IAdamItem interface properties
            Name = f.Name;
            Url = f.Url;
        }

    }

}