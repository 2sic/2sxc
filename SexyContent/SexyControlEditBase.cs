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
        protected void Page_Init(object sender, EventArgs e)
        {
            // Set ZoneId based on the context
            var zoneId = (!UserInfo.IsSuperUser
                           ? ZoneHelpers.GetZoneID(ModuleConfiguration.OwnerPortalID)
                           : ZoneHelpers.GetZoneID(!string.IsNullOrEmpty(Request.QueryString["ZoneId"])
                               ? int.Parse(Request.QueryString["ZoneId"])
                               : ModuleConfiguration.OwnerPortalID));

            // Set AppId based on the context
            var appId = AppHelpers.GetAppIdFromModule(ModuleConfiguration);

            // possibly get app-id from url, but only if on an admin-control
            // the only reason the code is currently here, is because the admin controls will be removed soon (replaced by JS-UIs)
            if (this is SexyControlAdminBaseWillSoonBeRemoved)
            {
                var appIdString = Request.QueryString[SexyContent.Settings.AppIDString];
                int appId2;
                if (appIdString != null && int.TryParse(appIdString, out appId2))
                    appId = appId2;
            }

            // Init SxcContext based on zone/app
            //if (zoneId.HasValue && appId.HasValue)
                SxcContext = new InstanceContext(zoneId.Value, appId.Value, true, ModuleConfiguration.OwnerPortalID,
                    ModuleContext.Configuration);
        }


        #region basic properties like Sexy, App, Zone, etc.
        protected InstanceContext SxcContext { get; set; }

        protected int? ZoneId => SxcContext?.ZoneId;

        protected int? AppId => SxcContext?.AppId;

        #endregion

        public bool IsContentApp => SxcContext.IsContentApp;// ModuleConfiguration.DesktopModule.ModuleName == "2sxc";

        // private ContentGroup _contentGroup;
        protected ContentGroup ContentGroup => SxcContext.ContentGroup; //  _contentGroup ?? (_contentGroup = SxcContext.ContentGroupManager.GetContentGroupForModule(ModuleConfiguration.ModuleID));

        #region template loading and stuff...

        protected Template Template => SxcContext.Template;

        //private Template _template;
        //protected Template Template
        //{
        //    get
        //    {
        //        if (!AppId.HasValue)
        //            return null;

        //        if (_template == null)
        //        {
        //            // Change Template if URL contains "ViewNameInUrl"
        //            if (!IsContentApp)
        //            {
        //                var urlParams = Request.QueryString;
        //                var templateFromUrl = TryToGetTemplateBasedOnUrlParams(urlParams);
        //                if (templateFromUrl != null)
        //                    _template = templateFromUrl;
        //            }

        //            if (_template == null)
        //                _template = ContentGroup.Template;
        //        }

        //        return _template;
        //    }
        //}

        ///// <summary>
        ///// combine all QueryString Params to a list of key/value lowercase and search for a template having this ViewNameInUrl
        ///// QueryString is never blank in DNN so no there's no test for it
        ///// </summary>
        //private Template TryToGetTemplateBasedOnUrlParams(NameValueCollection urlParams)
        //{
        //    var urlParameterDict = urlParams.AllKeys.ToDictionary(key => key?.ToLower() ?? "", key => string.Format("{0}/{1}", key, Request.QueryString[key]).ToLower());
        //    //var queryStringPairs = Request.QueryString.AllKeys.Select(key => string.Format("{0}/{1}", key, Request.QueryString[key]).ToLower()).ToArray();
        //    // var queryStringKeys = Request.QueryString.AllKeys.Select(k => k?.ToLower() ?? "").ToArray();

        //    foreach (var template in SxcContext.AppTemplates.GetAllTemplates().Where(t => !string.IsNullOrEmpty(t.ViewNameInUrl)))
        //    {
        //        var desiredFullViewName = template.ViewNameInUrl.ToLower();
        //        if (desiredFullViewName.EndsWith("/.*"))   // match details/.* --> e.g. details/12
        //        {
        //            var keyName = desiredFullViewName.Substring(0, desiredFullViewName.Length - 3);
        //            if (urlParameterDict.ContainsKey(keyName))
        //                return template;
        //        }
        //        else if (urlParameterDict.ContainsValue(desiredFullViewName)) // match view/details
        //            return template;

        //        //var viewNameInUrlLowered = template.ViewNameInUrl.ToLower();
        //        //if (queryStringPairs.Contains(viewNameInUrlLowered))    // match view/details
        //        //    return template;
        //        //if (viewNameInUrlLowered.EndsWith("/.*"))   // match details/.* --> e.g. details/12
        //        //{
        //        //    var keyName = viewNameInUrlLowered.Substring(0, viewNameInUrlLowered.Length - 3);
        //        //    if (queryStringKeys.Contains(keyName))
        //        //        return template;
        //        //}
        //    }

        //    return null;
        //}

        #endregion




    }
}