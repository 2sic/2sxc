using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.UI.Modules;

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
			if (UserMayEditThisModule && Parent is ModuleHost)
				RegisterGlobalsAttribute();
            }

        private SexyContent _sexy;
        protected SexyContent Sexy
        {
            get
            {
                if (_sexy == null && ZoneId.HasValue && AppId.HasValue)
                    _sexy = new SexyContent(ZoneId.Value, AppId.Value, true, ModuleConfiguration.OwnerPortalID);
                return _sexy;
            }
        }

        private SexyContent _sexyUncached;
        protected SexyContent SexyUncached
        {
            get
            {
                if (_sexyUncached == null && ZoneId.HasValue && AppId.HasValue)
                    _sexyUncached = new SexyContent(ZoneId.Value, AppId.Value, false, ModuleConfiguration.OwnerPortalID);
                return _sexyUncached;
            }
        }

        protected int? ZoneId
        {
            get
            {
                return SexyContent.GetZoneID(ModuleConfiguration.OwnerPortalID);
            }
        }

        protected virtual int? AppId
        {
            get
            {
                return SexyContent.GetAppIdFromModule(ModuleConfiguration);
            }
            set { SexyContent.SetAppIdForModule(ModuleConfiguration, value); }
        }


        public bool IsContentApp
        {
            get { return ModuleConfiguration.DesktopModule.ModuleName == "2sxc"; }
        }


        /// <summary>
        /// Holds the List of Elements for the current module.
        /// </summary>
		private List<Element> _elements;
        protected List<Element> Elements
        {
            get
            {
				if (_elements == null)
                {
					_elements = Sexy.GetContentElements(ModuleId, SexyContent.HasEditPermission(this.ModuleConfiguration), Template).ToList();
                }
				return _elements;
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
                        _template = Sexy.GetTemplateForModule(this.ModuleConfiguration.ModuleID);
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
            var queryStringPairs = Request.QueryString.AllKeys.Select(key => string.Format("{0}/{1}", key, Request.QueryString[key]).ToLower());
            var matchedTemplate = Sexy.TemplateContext.GetAllTemplates().FirstOrDefault(t => t.AppID == AppId && !string.IsNullOrEmpty(t.ViewNameInUrl) && queryStringPairs.Contains(t.ViewNameInUrl.ToLower()));
            return matchedTemplate;
        }

        protected bool IsList
        {
            get { return Template != null && Template.UseForList; }
        }

        protected bool UserMayEditThisModule
        {
            get
            {
                return ModuleContext.IsEditable;
            }
        }

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
			((ModuleHost)Parent).Attributes.Add("data-2sxc-globals", Newtonsoft.Json.JsonConvert.SerializeObject(new
			{
				ModuleContext = new
				{
					ModuleContext.PortalId,
					ModuleContext.TabId,
					ModuleContext.ModuleId,
					AppId
				},
				PortalSettings.ActiveTab.FullUrl,
				ApplicationPath = (Request.IsSecureConnection ? "https://" : "http://") + PortalAlias.HTTPAlias + "/",
				DefaultLanguageID = Sexy != null ? Sexy.ContentContext.GetLanguageId(PortalSettings.DefaultLanguage) : null
			}));
		}
    }
}