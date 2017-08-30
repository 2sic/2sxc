using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using Newtonsoft.Json;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Interfaces;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Installer;
using ToSic.SexyContent.Interfaces;

namespace ToSic.SexyContent
{
    /// <summary>
    /// This is an instance-context of a Content-Module. It basically encapsulates the instance-state, incl.
    /// IDs of Zone & App, the App, EAV-Context, Template, Content-Groups (if available), Environment and OriginalModule (in case it's from another portal)
    /// It is needed for just about anything, because without this set of information
    /// it would be hard to get anything done .
    /// Note that it also adds the current-user to the state, so that the system can log data-changes to this user
    /// </summary>
    public class SxcInstance :ISxcInstance
    {
        #region App-level information

        // 2017-04-01 2dm - disabled, to remove deep dependencies
        ///// <summary>
        ///// The Content Data Context pointing to a full EAV, pre-configured for this specific App
        ///// </summary>
        //public EavDataController EavAppContext => App.EavContext;

        internal int? ZoneId => ContentBlock.ZoneId;

        internal int? AppId => ContentBlock.AppId;

        public App App => ContentBlock.App;

        public bool IsContentApp => ContentBlock.IsContentApp;

        // 2016-09-16 deprecated these two shortcuts as mentioned in the todo, found ca. 3 + 4 use cases
        //public TemplateManager AppTemplates => App.TemplateManager; // todo: remove, use App...
        //public ContentGroupManager AppContentGroups => App.ContentGroupManager; // todo: remove, use App...

        #endregion

        /// <summary>
        /// The url-parameters (or alternative thereof) to use when picking views or anything
        /// Note that it's not the same type as the request.querystring to ease migration to future coding conventions
        /// </summary>
        internal IEnumerable<KeyValuePair<string, string>> Parameters;

        #region Info for current runtime instance
        public ContentGroup ContentGroup => ContentBlock.ContentGroup;


        /// <summary>
        /// Environment - should be the place to refactor everything into, which is the host around 2sxc
        /// </summary>
        public Environment.DnnEnvironment Environment = new Environment.DnnEnvironment();

        internal ModuleInfo ModuleInfo { get; }

        internal IContentBlock ContentBlock { get; }


        /// <summary>
        /// This returns the PS of the original module. When a module is mirrored across portals,
        /// then this will be different from the PortalSettingsOfVisitedPage, otherwise they are the same
        /// </summary>
        internal PortalSettings AppPortalSettings => ContentBlock.PortalSettings; // maybe pass in

        public ViewDataSource Data => ContentBlock.Data;


        #endregion

        #region  template helpers 

        public Template Template
        {
            get { return ContentBlock.Template; }
            set { ContentBlock.Template = value; }
        }

        private void CheckTemplateOverrides()
        {
            // skif if not relevant or not yet initialized
            if (IsContentApp || App == null) return;

            // #2 Change Template if URL contains the part in the metadata "ViewNameInUrl"
            if (Parameters == null) return;
            var templateFromUrl = TryToGetTemplateBasedOnUrlParams();
            if (templateFromUrl != null)
                Template = templateFromUrl;
        }

        private Template TryToGetTemplateBasedOnUrlParams()
        {
            if (Parameters == null) return null;

            // new 2016-05-01
            var urlParameterDict = Parameters.ToDictionary(pair => pair.Key?.ToLower() ?? "", pair =>
                $"{pair.Key}/{pair.Value}".ToLower());
            
            // old
            //var urlParameterDict = Parameters.AllKeys.ToDictionary(key => key?.ToLower() ?? "", key =>
            //    $"{key}/{Parameters[key]}".ToLower());

            foreach (var template in App.TemplateManager.GetAllTemplates().Where(t => !string.IsNullOrEmpty(t.ViewNameInUrl)))
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
            }

            return null;
        }

        #endregion

        #region Constructor
        internal SxcInstance(IContentBlock cb, ModuleInfo runtimeModuleInfo, IEnumerable<KeyValuePair<string, string>> urlparams = null)
        {
            ContentBlock = cb;
            ModuleInfo = runtimeModuleInfo;

            // try to get url parameters, because we may need them later for view-switching and more
            Parameters = urlparams;
            //if (Parameters == null)
            //    Parameters = HttpContext.Current?.Request?.QueryString; // todo: reduce dependency on context-current...
            // Build up the environment. If we know the module context, then use permissions from there
            // modinfo is null in cases where things are not known yet, portalsettings are null in search-scenarios
            Environment.Permissions = (ModuleInfo != null) && (PortalSettings.Current != null)
                ? (IPermissions)new Permissions(ModuleInfo)
                : new Environment.None.Permissions();

            // url-override of view / data
            CheckTemplateOverrides();   // allow view change on apps
        }

        internal SxcInstance(IContentBlock cb, SxcInstance runtimeInstance)
        {
            ContentBlock = cb;
            // Inherit various context information from the original SxcInstance
            ModuleInfo = runtimeInstance.ModuleInfo;
            Parameters = runtimeInstance.Parameters;
            Environment.Permissions = runtimeInstance.Environment.Permissions;

            // url-override of view / data
            CheckTemplateOverrides();   // allow view change on apps
        }
        #endregion

        #region RenderEngine
        internal bool RenderWithDiv = true;
        private bool RenderWithEditMetadata => Environment.Permissions.UserMayEditContent;
        public HtmlString Render()
        {
            var renderHelp = new RenderingHelpers(this);
            
            try
            {
                string innerContent = null;

                #region do pre-check to see if system is stable & ready
                var notReady = new InstallationController().CheckUpgradeMessage(PortalSettings.Current.UserInfo.IsSuperUser);
                if (!string.IsNullOrEmpty(notReady))
                    innerContent = renderHelp.DesignErrorMessage(new Exception(notReady), true, "Error - needs admin to fix this", false, false);

                #endregion

                #region check if the content-group exists (sometimes it's missing if a site is being imported and the data isn't in yet
                if (innerContent == null)
                {
                    if (ContentBlock.DataIsMissing)
                    {
                        if (Environment.Permissions.UserMayEditContent)
                        {
                            innerContent = ""; // stop further processing
                        }
                        else // end users should see server error as no js-side processing will happen
                        {
                            var ex =
                                new Exception(
                                    "Data is missing - usually when a site is copied but the content / apps have not been imported yet - check 2sxc.org/help?tag=export-import");
                            innerContent = renderHelp.DesignErrorMessage(ex, true,
                                "Error - needs admin to fix", false, true);
                        }
                    }
                }
                #endregion

                #region try to render the block or generate the error message
                if (innerContent == null)
                    try
                    {
                        if (Template != null) // when a content block is still new, there is no definition yet
                        {
                            var engine = GetRenderingEngine(InstancePurposes.WebView);
                            innerContent = engine.Render();
                        }
                        else innerContent = "";
                    }
                    catch (Exception ex)
                    {
                        innerContent = renderHelp.DesignErrorMessage(ex, true, "Error rendering template", false, true);
                    }
                #endregion

                #region Wrap it all up into a nice wrapper tag
                var editInfos = JsonConvert.SerializeObject(renderHelp.GetClientInfosAll());
                var isInnerContent = ContentBlock.ParentId != ContentBlock.ContentBlockId;
                var startTag = (RenderWithDiv
                    ? $"<div class=\"sc-viewport sc-content-block\" data-cb-instance=\"{ContentBlock.ParentId}\" " +
                      $" data-cb-id=\"{ContentBlock.ContentBlockId}\""
                      +
                      (RenderWithEditMetadata
                          ? " data-edit-context=\'" + editInfos + "'"
                          : "")
                      + ">\n"
                    : "");
                var endTag = (RenderWithDiv ? "\n</div>" : "");
                string result = startTag + innerContent + endTag;
                #endregion

                return new HtmlString(result);
            }
            catch (Exception ex)
            {
                // todo: i18n
                return new HtmlString(renderHelp.DesignErrorMessage(ex, true, null, true, true));
            }
        }

        public IEngine GetRenderingEngine(InstancePurposes renderingPurpose)
        {
            var engine = EngineFactory.CreateEngine(Template);
            engine.Init(Template, App, ModuleInfo, Data, renderingPurpose, this);
            return engine;
        }

        #endregion
    }
}