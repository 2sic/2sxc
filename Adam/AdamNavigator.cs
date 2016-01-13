using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Services.FileSystem;
using ToSic.Eav;

namespace ToSic.SexyContent.Adam
{
    public class AdamNavigator : AdamFolder
    {
        public AdamNavigator(SexyContent sexy, App app, Razor.Helpers.DnnHelper dnn, Guid entityGuid, string fieldName)
        {
            Core = new Core(sexy, app, dnn, entityGuid, fieldName);
            //App = app;

            if (!Exists)
                return;

            var f = Core.Get(Root) as FolderInfo;

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
        }

        public string Root => Core.GeneratePath("");

        public bool Exists => Core.Exists(Root);

    }

}