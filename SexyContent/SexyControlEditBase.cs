using System;
using System.Collections.Specialized;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.UI.Modules;
using Newtonsoft.Json;
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
            if ((UserMayEditThisModule || this is SexyControlAdminBase) && Parent is ModuleHost)
                RegisterGlobalsAttribute();
        }

        #region basic properties like Sexy, App, Zone, etc.
        private InstanceContext _sxcContext;
        protected InstanceContext SxcContext
        {
            get
            {
                if (_sxcContext == null && ZoneId.HasValue && AppId.HasValue)
                    _sxcContext = new InstanceContext(ZoneId.Value, AppId.Value, true, ModuleConfiguration.OwnerPortalID, ModuleContext.Configuration);
                return _sxcContext;
            }
        }

        // todo: refactor
        // + apparently it allows zone-change in the URL if the user is a super-user, which is probably only used in admin-views
        private int? _zoneId;
        protected int? ZoneId
        {
            get
            {
                return _zoneId ??
                       (_zoneId = (!UserInfo.IsSuperUser
                           ? ZoneHelpers.GetZoneID(ModuleConfiguration.OwnerPortalID)
                           : ZoneHelpers.GetZoneID(!string.IsNullOrEmpty(Request.QueryString["ZoneId"])
                               ? int.Parse(Request.QueryString["ZoneId"])
                               : ModuleConfiguration.OwnerPortalID)));
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
                    _cachedAppId = AppHelpers.GetAppIdFromModule(ModuleConfiguration);
                    _appIdCached = true;
                }
                return _cachedAppId;
            }
        }
        #endregion

        public bool IsContentApp => SxcContext.IsContentApp;// ModuleConfiguration.DesktopModule.ModuleName == "2sxc";

        // private ContentGroup _contentGroup;
        protected ContentGroup ContentGroup => SxcContext.ContentGroup; //  _contentGroup ?? (_contentGroup = SxcContext.ContentGroupManager.GetContentGroupForModule(ModuleConfiguration.ModuleID));

        private Template _template;
        protected Template Template
        {
            get
            {
                if (!AppId.HasValue)
                    return null;

                if (_template == null)
                {
                    // Change Template if URL contains "ViewNameInUrl"
                    if (!IsContentApp)
                    {
                        var urlParams = Request.QueryString;
                        var templateFromUrl = TryToGetTemplateBasedOnUrlParams(urlParams);
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
        private Template TryToGetTemplateBasedOnUrlParams(NameValueCollection urlParams)
        {
            var urlParameterDict = urlParams.AllKeys.ToDictionary(key => key?.ToLower() ?? "", key => string.Format("{0}/{1}", key, Request.QueryString[key]).ToLower());
            //var queryStringPairs = Request.QueryString.AllKeys.Select(key => string.Format("{0}/{1}", key, Request.QueryString[key]).ToLower()).ToArray();
            // var queryStringKeys = Request.QueryString.AllKeys.Select(k => k?.ToLower() ?? "").ToArray();

            foreach (var template in SxcContext.AppTemplates.GetAllTemplates().Where(t => !string.IsNullOrEmpty(t.ViewNameInUrl)))
            {
                var desiredFullViewName = template.ViewNameInUrl.ToLower();
                if (desiredFullViewName.EndsWith("/.*"))   // match details/.* --> e.g. details/12
                {
                    var keyName = desiredFullViewName.Substring(0, desiredFullViewName.Length - 3);
                    if (urlParameterDict.ContainsKey(keyName))
                        return template;
                }
                else if (urlParameterDict.ContainsValue(desiredFullViewName)) // match view/details
                    return template;

                //var viewNameInUrlLowered = template.ViewNameInUrl.ToLower();
                //if (queryStringPairs.Contains(viewNameInUrlLowered))    // match view/details
                //    return template;
                //if (viewNameInUrlLowered.EndsWith("/.*"))   // match details/.* --> e.g. details/12
                //{
                //    var keyName = viewNameInUrlLowered.Substring(0, viewNameInUrlLowered.Length - 3);
                //    if (queryStringKeys.Contains(keyName))
                //        return template;
                //}
            }

            return null;
        }

        protected bool IsList => Template != null && Template.UseForList;

        private InstanceContext _sexyForSecurityCheck;
        /// <summary>
        ///  If Sexy is null, instanciate new SexyContent(ZoneId, 0), otherwise return the existing;
        /// </summary>
        private InstanceContext InstanceForSecurityCheck => _sexyForSecurityCheck ?? (_sexyForSecurityCheck = (SxcContext == null
            ? new InstanceContext(ZoneId.Value, 0, true, ModuleConfiguration.OwnerPortalID, ModuleConfiguration)
            : SxcContext));

        protected bool UserMayEditThisModule => InstanceForSecurityCheck?.Environment?.Permissions.UserMayEditContent ?? false;

        protected bool StandAlone => Request.QueryString["standalone"] == "true";

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
                DefaultLanguageID = SxcContext != null ? SxcContext.EavAppContext.Dimensions.GetLanguageId(PortalSettings.DefaultLanguage) : null
            }));
        }

    }
}