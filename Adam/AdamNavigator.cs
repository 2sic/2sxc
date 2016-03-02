using System;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.FileSystem;

namespace ToSic.SexyContent.Adam
{
    public class AdamNavigator : AdamFolder
    {
        public AdamNavigator(SxcInstance sexy, App app, PortalSettings ps, Guid entityGuid, string fieldName)
        {
            EntityBase = new EntityBase(sexy, app, ps, entityGuid, fieldName);
            Manager = new AdamManager(ps.PortalId, app);

            if (!Exists)
                return;

            var f = Manager.Get(Root) as FolderInfo;

            if (f == null)
                return;

            PortalID = f.PortalID;
            FolderPath = f.FolderPath;
            MappedPath = f.MappedPath;
            StorageLocation = f.StorageLocation;
            IsProtected = f.IsProtected;
            IsCached = f.IsCached;
            FolderMappingID = f.FolderMappingID;
            LastUpdated = f.LastUpdated;
            FolderID = f.FolderID;
            DisplayName = f.DisplayName;
            DisplayPath = f.DisplayPath;
            IsVersioned = f.IsVersioned;
            KeyID = f.KeyID;
            ParentID = f.ParentID;
            UniqueId = f.UniqueId;
            VersionGuid = f.VersionGuid;
            WorkflowID = f.WorkflowID;

            // IAdamItem interface properties
            Name = DisplayName;
        }

        public string Root => EntityBase.EntityRoot;

        public bool Exists => Manager.Exists(Root);

    }

}