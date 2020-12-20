using System;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// The ADAM Navigator creates a folder object for an entity/field combination
    /// This is the root folder where all files for this field are stored
    /// </summary>
    public class FolderOfField<TFolderId, TFileId> : Folder<TFolderId, TFileId>
    {
        //public ContainerBase AdamOfField { get; set; }
        public FolderOfField(AdamManager<TFolderId, TFileId> adamContext, Guid entityGuid, string fieldName) 
            : base(adamContext)
        {
            var adamOfField = new AdamOfField<TFolderId, TFileId>(AdamContext, entityGuid, fieldName);

            if (!AdamContext.Exists(adamOfField.Root)) return;

            var f = AdamContext.Folder(adamOfField.Root);
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

        // 2020-09-12 2dm removed, I don't think it's surfaced anywhere
        // and the only public use is an IFolder, so it shouldn't be seen elsewhere
        //private bool Exists => AdamContext.Exists(AdamOfField.Root);

    }

}