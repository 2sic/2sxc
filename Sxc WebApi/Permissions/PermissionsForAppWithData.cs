using System;
using System.Collections.Generic;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Formats;

namespace ToSic.SexyContent.WebApi.Permissions
{
    internal class PermissionsForAppWithData: PermissionsForAppAndTypes
    {
        public PermissionsForAppWithData(SxcInstance sxcInstance, int appId, List<ItemIdentifier> items, Log parentLog) 
            : base(sxcInstance, appId, items, parentLog)
        {
            InitAppData();
        }

        private void InitAppData()
        {
            if (SxcInstance?.Data == null)
                throw new Exception("Can't use app-data at the moment, because it requires an instance context");

            var showDrafts = PermissionChecker.UserMay(GrantSets.ReadDraft);

            App.InitData(showDrafts,
                SxcInstance.Environment.PagePublishing.IsEnabled(SxcInstance.EnvInstance.Id),
                SxcInstance.Data.ConfigurationProvider);
        }

    }
}
