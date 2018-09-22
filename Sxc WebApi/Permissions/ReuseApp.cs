using System;
using ToSic.Eav.Security.Permissions;

namespace ToSic.SexyContent.WebApi.Permissions
{
    internal static class ReuseApp
    {
        internal static void InitializeData(this MultiPermissionsApp mpa)
        {
            if (mpa.SxcInstance?.Data == null)
                throw new Exception("Can't use app-data at the moment, because it requires an instance context");

            var showDrafts = mpa.UserMayOnAll(GrantSets.ReadDraft);

            mpa.App.InitData(showDrafts,
                mpa.SxcInstance.Environment.PagePublishing.IsEnabled(mpa.SxcInstance.EnvInstance.Id),
                mpa.SxcInstance.Data.ConfigurationProvider);
        }
    }
}
