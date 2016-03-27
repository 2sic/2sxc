using System;
using System.Collections.Specialized;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.UI.Modules;
using Newtonsoft.Json;
using ToSic.SexyContent.Administration;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent
{
    /// <summary>
    /// Contains properties that all controls use that edit the current module's data (not global data like admin controls)
    /// It delivers a context that uses the current modules App and the current portal's Zone.
    /// </summary>
    public abstract class SexyControlEditBase : PortalModuleBase
    {
        public ModuleContentBlock ContentBlock;
        protected void Page_Init(object sender, EventArgs e)
        {
            ContentBlock = new ModuleContentBlock(ModuleConfiguration, UserInfo, Request.QueryString, UserInfo.IsSuperUser);
            // Set ZoneId based on the context
            //var zoneId = (!UserInfo.IsSuperUser
            //               ? ZoneHelpers.GetZoneID(ModuleConfiguration.OwnerPortalID)
            //               : !string.IsNullOrEmpty(Request.QueryString["ZoneId"])
            //                   ? int.Parse(Request.QueryString["ZoneId"]) : ZoneHelpers.GetZoneID(ModuleConfiguration.OwnerPortalID));

            //// Set AppId based on the context
            //var appId = AppHelpers.GetAppIdFromModule(ModuleConfiguration);
            //if (appId != null)
            //    SettingsAreStored = true;

            // possibly get app-id from url, but only if on an admin-control
            // the only reason the code is currently here, is because the admin controls will be removed soon (replaced by JS-UIs)
            // 2016-03-23 2dm commented this all out, as the request-string is already checked in the AppHelpers.GetAppIdFromModule(...)
            //if (this is SexyControlAdminBaseWillSoonBeRemoved)
            //{
            //    var appIdString = Request.QueryString[SexyContent.Settings.AppIDString];
            //    int appId2;
            //    if (appIdString != null && int.TryParse(appIdString, out appId2))
            //        appId = appId2;
            //}

            // Init SxcContext based on zone/app
            //if (zoneId.HasValue && appId.HasValue)
                //_sxcInstance = new SxcInstance(ContentBlock.ZoneId ?? 0, ContentBlock.AppId ?? 0, ModuleConfiguration.OwnerPortalID,
                //    ModuleConfiguration);
        }


        #region basic properties like Sexy, App, Zone, etc.

        protected SxcInstance _sxcInstance => ContentBlock.SxcInstance;

        protected int? ZoneId => _sxcInstance?.ZoneId ?? 0;

        protected int? AppId => SettingsAreStored ? _sxcInstance?.AppId : null; // some systems rely on appid being blank to adapt behaviour if nothing is saved yet

        protected bool SettingsAreStored => ContentBlock.UnreliableInfoThatSettingsAreStored;
        #endregion

        public bool IsContentApp => _sxcInstance.IsContentApp;

        // private ContentGroup _contentGroup;
        protected ContentGroup ContentGroup => _sxcInstance.ContentGroup; 

        #region template loading and stuff...

        protected Template Template => _sxcInstance.Template;

        #endregion




    }
}