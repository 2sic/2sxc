using System;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// The ADAM Navigator creates a folder object for an entity/field combination
    /// This is the root folder where all files for this field are stored
    /// </summary>
    public class FolderOfField : Folder<int, int>
    {
        protected ContainerBase AdamOfField { get; set; }
        public FolderOfField(AdamAppContext<int, int> adamContext, Guid entityGuid, string fieldName) 
            : base(adamContext)
        {
            AdamOfField = new AdamOfField<int, int>(AdamContext, entityGuid, fieldName);

            if (!Exists) return;

            var f = AdamContext.Folder(AdamOfField.Root);
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

        public bool Exists => AdamContext.Exists(AdamOfField.Root);

    }

}