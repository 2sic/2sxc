using System;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// The ADAM Navigator creates a folder object for an entity/field combination
    /// This is the root folder where all files for this field are stored
    /// </summary>
    public class FolderOfField : Folder<int, int>
    {
        protected ContainerOfField Container { get; set; }
        public FolderOfField(AdamAppContext adamContext, Guid entityGuid, string fieldName) 
            : base(adamContext)
        {
            Container = new ContainerOfField(AdamContext, entityGuid, fieldName);

            if (!Exists) return;

            // ReSharper disable once PatternAlwaysOfType
            //if (!(AdamContext.Folder(Container.Root) is Eav.Apps.Assets.IFolder f))
            //    return;
            var f = AdamContext.Folder(Container.Root);
            if (f == null) return;

            Path = f.Path;
            Modified = f.Modified;
            SysId = (f as Folder<int, int>).SysId;
            Created = f.Created;
            Modified = f.Modified;

            // IAdamItem interface properties
            Name = f.Name;
            Url = f.Url;
        }

        public bool Exists => AdamContext.Exists(Container.Root);

    }

}