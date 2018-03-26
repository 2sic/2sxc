using System;
using ToSic.Eav.Apps.Assets;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// The ADAM Navigator creates a folder object for an entity/field combination
    /// This is the root folder where all files for this field are stored
    /// </summary>
    public class AssetFolderOfField : AssetFolder
    {
        protected ContainerOfField Container { get; set; }
        public AssetFolderOfField(IEnvironmentFileSystem enfFileSystem, AdamAppContext appContext, Guid entityGuid, string fieldName) : base(appContext, enfFileSystem)
        {
            Container = new ContainerOfField(AppContext, entityGuid, fieldName);

            if (!Exists)
                return;

            // ReSharper disable once PatternAlwaysOfType
            if (!(AppContext.Folder(Container.Root) is Folder f))
                return;

            FolderPath = f.FolderPath;
            LastUpdated = f.LastUpdated;
            Id = f.Id;
            CreatedOnDate = f.CreatedOnDate;
            LastUpdated = f.LastUpdated;

            // IAdamItem interface properties
            Name = f.Name;
        }

        //public string Root => Container.Root;

        public bool Exists => AppContext.Exists(Container.Root);

    }

}