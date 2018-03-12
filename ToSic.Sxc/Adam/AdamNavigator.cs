using System;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Apps.Interfaces;

namespace ToSic.SexyContent.Adam
{
    /// <summary>
    /// The ADAM Navigator creates a folder object for an entity/field combination
    /// This is the root folder where all files for this field are stored
    /// </summary>
    public class AdamNavigator : AdamFolder
    {
        public AdamNavigator(IEnvironmentFileSystem enfFs, SxcInstance sexy, App app, ITenant tenant, Guid entityGuid, string fieldName, bool usePortalRoot) : base(enfFs)
        {
            AdamBrowseContext = new AdamBrowseContext(sexy, app, tenant, entityGuid, fieldName, usePortalRoot);
            Manager = AdamBrowseContext.AdamManager;

            if (!Exists)
                return;

            // ReSharper disable once PatternAlwaysOfType
            if (!(Manager.Get(Root) is Folder f))
                return;

            FolderPath = f.FolderPath;
            LastUpdated = f.LastUpdated;
            Id = f.Id;
            CreatedOnDate = f.CreatedOnDate;
            LastUpdated = f.LastUpdated;

            // IAdamItem interface properties
            Name = f.Name;

            // commented out stuff is from DNN
            // but it will probably never be cross-platform
            //Name = f.DisplayName;            //DisplayName = f.DisplayName;
            //DisplayPath = f.DisplayPath;
            //PortalID = f.PortalID;
            //MappedPath = f.MappedPath;
            //StorageLocation = f.StorageLocation;
            //IsProtected = f.IsProtected;
            //IsCached = f.IsCached;
            //FolderMappingID = f.FolderMappingID;
            //IsVersioned = f.IsVersioned;
            //KeyID = f.KeyID;
            //ParentID = f.ParentID;
            //UniqueId = f.UniqueId;
            //VersionGuid = f.VersionGuid;
            //WorkflowID = f.WorkflowID;

        }

        public string Root => AdamBrowseContext.EntityRoot;

        public bool Exists => Manager.Exists(Root);

    }

}