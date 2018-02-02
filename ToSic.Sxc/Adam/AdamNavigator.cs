using System;
using DotNetNuke.Services.FileSystem;
using ToSic.Eav.Apps.Interfaces;

namespace ToSic.SexyContent.Adam
{
    public class AdamNavigator : AdamFolder
    {
        public AdamNavigator(SxcInstance sexy, App app, ITennant tennant, Guid entityGuid, string fieldName, bool usePortalRoot)
        {
            EntityBase = new EntityBase(sexy, app, tennant, entityGuid, fieldName, usePortalRoot);
            Manager = new AdamManager(tennant.Id, app);

            if (!Exists)
                return;

            if (!(Manager.Get(Root) is FolderInfo f))
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
//            CreatedOnDate = f.CreatedOnDate;

            // IAdamItem interface properties
            Name = DisplayName;
        }

        public string Root => EntityBase.EntityRoot;

        public bool Exists => Manager.Exists(Root);

    }

}