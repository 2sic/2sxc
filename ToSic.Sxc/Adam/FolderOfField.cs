using System;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// The ADAM Navigator creates a folder object for an entity/field combination
    /// This is the root folder where all files for this field are stored
    /// </summary>
    public class FolderOfField : Folder
    {
        protected ContainerOfField Container { get; set; }
        public FolderOfField(IEnvironmentFileSystem enfFileSystem, AdamAppContext appContext, Guid entityGuid, string fieldName) : base(appContext, enfFileSystem)
        {
            Container = new ContainerOfField(AppContext, entityGuid, fieldName);

            if (!Exists)
                return;

            // ReSharper disable once PatternAlwaysOfType
            if (!(AppContext.Folder(Container.Root) is Eav.Apps.Assets.Folder f))
                return;

            FolderPath = f.FolderPath;
            Modified = f.Modified;
            Id = f.Id;
            Created = f.Created;
            Modified = f.Modified;

            // IAdamItem interface properties
            Name = f.Name;
        }

        //public string Root => Container.Root;

        public bool Exists => AppContext.Exists(Container.Root);

    }

}