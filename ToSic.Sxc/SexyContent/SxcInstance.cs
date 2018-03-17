using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Interfaces;
using ToSic.Sxc.Interfaces;

namespace ToSic.SexyContent
{
    /// <summary>
    /// This is an instance-context of a Content-Module. It basically encapsulates the instance-state, incl.
    /// IDs of Zone and App, the App, EAV-Context, Template, Content-Groups (if available), Environment and OriginalModule (in case it's from another portal)
    /// It is needed for just about anything, because without this set of information
    /// it would be hard to get anything done .
    /// Note that it also adds the current-user to the state, so that the system can log data-changes to this user
    /// </summary>
    public class SxcInstance :ISxcInstance
    {
        #region App-level information

        internal int? ZoneId => ContentBlock.ZoneId;
        internal int? AppId => ContentBlock.AppId;
        public App App => ContentBlock.App;
        public bool IsContentApp => ContentBlock.IsContentApp;

        #endregion

        public Log Log { get; }

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
        public IEnvironment Environment { get; }

        public IEnvironmentFactory EnvFac { get; }

        public IInstanceInfo InstanceInfo { get; }

        internal IContentBlock ContentBlock { get; }


        /// <summary>
        /// This returns the PS of the original module. When a module is mirrored across portals,
        /// then this will be different from the PortalSettingsOfVisitedPage, otherwise they are the same
        /// </summary>
        internal ITenant Tenant => ContentBlock.Tenant;

        public ViewDataSource Data => ContentBlock.Data;


        #endregion

        #region  template helpers 

        public Template Template
        {
            get => ContentBlock.Template;
            set => ContentBlock.Template = value;
        }

        internal void SetTemplateOrOverrideFromUrl(Template defaultTemplate)
        {
            Template = defaultTemplate;
            // skif if not relevant or not yet initialized
            if (IsContentApp || App == null) return;

            // #2 Change Template if URL contains the part in the metadata "ViewNameInUrl"
            var templateFromUrl = TryToGetTemplateBasedOnUrlParams();
            if (templateFromUrl != null)
                Template = templateFromUrl;
        }

        private Template TryToGetTemplateBasedOnUrlParams()
        {
            Log.Add("template override - check");
            if (Parameters == null) return null;

            // new 2016-05-01
            var urlParameterDict = Parameters.ToDictionary(pair => pair.Key?.ToLower() ?? "", pair =>
                $"{pair.Key}/{pair.Value}".ToLower());
            

            foreach (var template in App.TemplateManager.GetAllTemplates().Where(t => !string.IsNullOrEmpty(t.ViewNameInUrl)))
            {
                var desiredFullViewName = template.ViewNameInUrl.ToLower();
                if (desiredFullViewName.EndsWith("/.*"))   // match details/.* --> e.g. details/12
                {
                    var keyName = desiredFullViewName.Substring(0, desiredFullViewName.Length - 3);
                    if (urlParameterDict.ContainsKey(keyName))
                    {
                        Log.Add("template override - found:" + template.Name);
                        return template;
                    }
                }
                else if (urlParameterDict.ContainsValue(desiredFullViewName)) // match view/details
                {
                    Log.Add("template override - found:" + template.Name);
                    return template;
                }
            }

            Log.Add("template override - none");
            return null;
        }

        #endregion

        #region Constructor
        internal SxcInstance(IContentBlock  cb, 
            IInstanceInfo runtimeModuleInfo, 
            IEnumerable<KeyValuePair<string, string>> urlparams = null, 
            Log parentLog = null)
        {
            Log = new Log("Sxc.Instnc", parentLog, $"get SxcInstance for a:{cb?.AppId} cb:{cb?.ContentBlockId}");
            EnvFac = Factory.Resolve<IEnvironmentFactory>();
            Environment = EnvFac.Environment(parentLog);
            ContentBlock = cb;
            InstanceInfo = runtimeModuleInfo;

            // keep url parameters, because we may need them later for view-switching and more
            Parameters = urlparams;
        }

        #endregion

        #region RenderEngine
        internal bool RenderWithDiv = true;
        public bool UserMayEdit => _userMayEdit 
            ?? (_userMayEdit = EnvFac.InstancePermissions(Log, InstanceInfo, App).UserMay(PermissionGrant.Full)).Value;
        private bool? _userMayEdit;

        public HtmlString Render()
        {
            Log.Add("render");
            var renderHelp = Factory.Resolve<IRenderingHelpers>().Init(this, Log);
            
            try
            {
                string body = null;

                #region do pre-check to see if system is stable & ready

                var installer = Factory.Resolve<IEnvironmentInstaller>();
                var notReady = installer.UpgradeMessages();
                if (!string.IsNullOrEmpty(notReady))
                {
                    Log.Add("system isn't ready,show upgrade message");
                    body = renderHelp.DesignErrorMessage(new Exception(notReady), true,
                        "Error - needs admin to fix this", false, false);
                }
                Log.Add("system is ready, no upgrade-message to show");

                #endregion

                #region check if the content-group exists (sometimes it's missing if a site is being imported and the data isn't in yet
                if (body == null)
                {
                    Log.Add("pre-init innerContent content is empty so no errors, will build");
                    if (ContentBlock.DataIsMissing)
                    {
                        Log.Add("content-block is missing data - will show error or just stop if not-admin-user");
                        if (UserMayEdit)
                            body = ""; // stop further processing
                        else // end users should see server error as no js-side processing will happen
                        {
                            var ex =
                                new Exception(
                                    "Data is missing - usually when a site is copied but the content / apps have not been imported yet - check 2sxc.org/help?tag=export-import");
                            body = renderHelp.DesignErrorMessage(ex, true,
                                "Error - needs admin to fix", false, true);
                        }
                    }
                }
                #endregion

                #region try to render the block or generate the error message
                if (body == null)
                    try
                    {
                        if (Template != null) // when a content block is still new, there is no definition yet
                        {
                            Log.Add("standard case, found template, will render");
                            var engine = GetRenderingEngine(InstancePurposes.WebView);
                            body = engine.Render();
                        }
                        else body = "";
                    }
                    catch (Exception ex)
                    {
                        body = renderHelp.DesignErrorMessage(ex, true, "Error rendering template", false, true);
                    }
                #endregion

                #region Wrap it all up into a nice wrapper tag
                var result = RenderWithDiv
                    ? renderHelp.WrapInContext(body,
                        instanceId: ContentBlock.ParentId,
                        contentBlockId: ContentBlock.ContentBlockId,
                        includeEditInfos: UserMayEdit)
                    : body;
                #endregion

                return new HtmlString(result);
            }
            catch (Exception ex)
            {
                return new HtmlString(renderHelp.DesignErrorMessage(ex, true, null, true, true));
            }
        }

        public IEngine GetRenderingEngine(InstancePurposes renderingPurpose)
        {
            var engine = EngineFactory.CreateEngine(Template);
            engine.Init(Template, App, InstanceInfo, Data, renderingPurpose, this, Log);
            return engine;
        }

        #endregion
    }
}