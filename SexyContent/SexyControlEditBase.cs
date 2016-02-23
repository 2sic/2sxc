using System;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.UI.Modules;
using Newtonsoft.Json;

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
            if ((UserMayEditThisModule || this is SexyControlAdminBase) && Parent is ModuleHost)
                RegisterGlobalsAttribute();
        }

        #region basic properties like Sexy, App, Zone, etc.
        private SexyContent _sexy;
        protected SexyContent Sexy
        {
            get
            {
                if (_sexy == null && ZoneId.HasValue && AppId.HasValue)
                    _sexy = new SexyContent(ZoneId.Value, AppId.Value, true, ModuleConfiguration.OwnerPortalID, ModuleContext.Configuration);
                return _sexy;
            }
        }

        protected int? ZoneId
        {
            get
            {
                return (!UserInfo.IsSuperUser ? SexyContent.GetZoneID(ModuleConfiguration.OwnerPortalID) :
                    (!String.IsNullOrEmpty(Request.QueryString["ZoneId"]) ? int.Parse(Request.QueryString["ZoneId"]) : SexyContent.GetZoneID(ModuleConfiguration.OwnerPortalID)));
            }
        }

        private int? _cachedAppId = null;
        private bool _appIdCached = false;
        protected virtual int? AppId
        {
            get
            {
                if (!_appIdCached)
                {
                    _cachedAppId = SexyContent.GetAppIdFromModule(ModuleConfiguration);
                    _appIdCached = true;
                }
                return _cachedAppId;

                // return SexyContent.GetAppIdFromModule(ModuleConfiguration);
            }
        }
        #endregion

        public bool IsContentApp
        {
            get { return ModuleConfiguration.DesktopModule.ModuleName == "2sxc"; }
        }

        private ContentGroup _contentGroup;

        protected ContentGroup ContentGroup
        {
            get
            {
                if (_contentGroup == null)
                    _contentGroup =
                        Sexy.ContentGroups.GetContentGroupForModule(ModuleConfiguration.ModuleID);
                return _contentGroup;
            }
        }

        private Template _template;
        protected Template Template
        {
            get
            {
                if (!AppId.HasValue)
                    return null;

                if (_template == null)
                {
                    // Change Template if URL contais "ViewNameInUrl"
                    if (!IsContentApp)
                    {
                        var templateFromUrl = GetTemplateFromUrl();
                        if (templateFromUrl != null)
                            _template = templateFromUrl;
                    }

                    if (_template == null)
                        _template = ContentGroup.Template;
                }

                return _template;
            }
        }

        /// <summary>
        /// combine all QueryString Params to a list of key/value lowercase and search for a template having this ViewNameInUrl
        /// QueryString is never blank in DNN so no there's no test for it
        /// </summary>
        private Template GetTemplateFromUrl()
        {
            var queryStringPairs = Request.QueryString.AllKeys.Select(key => string.Format("{0}/{1}", key, Request.QueryString[key]).ToLower()).ToArray();
            var queryStringKeys = Request.QueryString.AllKeys.Select(k => k == null ? "" : k.ToLower()).ToArray();

            foreach (var template in Sexy.Templates.GetAllTemplates().Where(t => !string.IsNullOrEmpty(t.ViewNameInUrl)))
            {
                var viewNameInUrlLowered = template.ViewNameInUrl.ToLower();
                if (queryStringPairs.Contains(viewNameInUrlLowered))    // match view/details
                    return template;
                if (viewNameInUrlLowered.EndsWith("/.*"))   // match details/.* --> e.g. details/12
                {
                    var keyName = viewNameInUrlLowered.Substring(0, viewNameInUrlLowered.Length - 3);
                    if (queryStringKeys.Contains(keyName))
                        return template;
                }
            }

            return null;
        }

        protected bool IsList
        {
            get { return Template != null && Template.UseForList; }
        }

        private SexyContent _sexyForSecurityCheck;
        private SexyContent SexyForSecurityCheck
        {
            get
            {
                // If Sexy is null, instanciate new SexyContent(ZoneId, 0);
                if (_sexyForSecurityCheck == null)
                    _sexyForSecurityCheck = (Sexy == null ? new SexyContent(ZoneId.Value, 0, true, ModuleConfiguration.OwnerPortalID, ModuleConfiguration) : Sexy);

                return _sexyForSecurityCheck;
            }
        }
        protected bool UserMayEditThisModule => SexyForSecurityCheck?.Environment?.Permissions.UserMayEditContent ?? false;

        //private bool? _userMayEdit = null;
        //protected bool UserMayEditThisModule
        //{
        //    get
        //    {
        //        if (_userMayEdit.HasValue)
        //            return _userMayEdit.Value;

        //        var okOnModule = DotNetNuke.Security.Permissions.ModulePermissionController.CanEditModuleContent(ModuleContext.Configuration);

        //        _userMayEdit = okOnModule;
        //        // if a user only has tab-edit but not module edit and is not admin, this needs additional confirmation (probably dnn bug)
        //        if(!okOnModule)
        //            _userMayEdit = DotNetNuke.Security.Permissions.TabPermissionController.HasTabPermission("EDIT");

        //        return _userMayEdit.Value;
        //        return ModuleContext.IsEditable;
        //    }
        //}

        protected bool StandAlone
        {
            get { return Request.QueryString["standalone"] == "true"; }
        }

        /// <summary>
        /// Add data-2sxc-globals Attribute to the DNN ModuleHost
        /// </summary>
        private void RegisterGlobalsAttribute()
        {
            // Add some required variables to module host div
            ((ModuleHost)Parent).Attributes.Add("data-2sxc-globals", JsonConvert.SerializeObject(new
            {
                ModuleContext = new
                {
                    ModuleContext.PortalId,
                    ModuleContext.TabId,
                    ModuleContext.ModuleId,
                    AppId
                },
                PortalSettings.ActiveTab.FullUrl,
                PortalRoot = (Request.IsSecureConnection ? "https://" : "http://") + PortalAlias.HTTPAlias + "/",
                DefaultLanguageID = Sexy != null ? Sexy.ContentContext.Dimensions.GetLanguageId(PortalSettings.DefaultLanguage) : null
            }));
        }

        // 2016-02-24
        ///// <summary>
        ///// Check different conditions (app/content) to determine if getting-started should be shown
        ///// </summary>
        ///// <returns></returns>
        //public bool ShowGettingStarted()
        //{
        //    if (IsContentApp)
        //    {
        //        var noTemplatesYet = !Sexy.Templates.GetVisibleTemplates().Any();
        //        return noTemplatesYet && IsEditable && UserInfo.IsInRole(PortalSettings.AdministratorRoleName);
        //    }
        //    else
        //    {
        //        return IsEditable && UserInfo.IsInRole(PortalSettings.AdministratorRoleName) &&
        //               !SexyContent.GetApps(ZoneId.Value, false, new PortalSettings(ModuleConfiguration.OwnerPortalID))
        //                   .Any();
        //    }
        //}
    }
}