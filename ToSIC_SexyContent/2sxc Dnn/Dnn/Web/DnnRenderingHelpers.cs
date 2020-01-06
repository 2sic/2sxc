using System;
using System.Web;
using System.Web.UI;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Client.ClientResourceManagement;
using Newtonsoft.Json;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Run;
using ToSic.Sxc.Dnn.Web.ClientInfos;
using ToSic.Sxc.Interfaces;

namespace ToSic.Sxc.Dnn.Web
{
    public class DnnRenderingHelpers : IHasLog, IRenderingHelpers
    {
        private Sxc.Blocks.ICmsBlock _cmsInstance;
        private PortalSettings _portalSettings;
        private UserInfo _userInfo;
        private string _applicationRoot;
        private IContainer _moduleInfo;

        public ILog Log { get; } = new Log("Dnn.Render");

        // Blank constructor for IoC
        public DnnRenderingHelpers() { }

        public DnnRenderingHelpers(Sxc.Blocks.ICmsBlock cms, ILog parentLog) => Init(cms, parentLog);

        public IRenderingHelpers Init(Sxc.Blocks.ICmsBlock cms, ILog parentLog)
        {
            this.LinkLog(parentLog);
            var appRoot = VirtualPathUtility.ToAbsolute("~/");
            _moduleInfo = cms?.Container;
            _cmsInstance = cms;
            _portalSettings = PortalSettings.Current;

            _userInfo = PortalSettings.Current.UserInfo;
            _applicationRoot = appRoot;

            return this;
        }



        public string WrapInContext(string content,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            int instanceId = 0, 
            int contentBlockId = 0, 
            bool editContext = false,
            string tag = Constants.DefaultContextTag,
            bool autoToolbar = false,
            bool addLineBreaks = true)
        {
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "ContextAttributes", $"{nameof(instanceId)},{nameof(contentBlockId)},{nameof(editContext)},{nameof(tag)},{nameof(autoToolbar)},{nameof(addLineBreaks)}");

            var contextAttribs = ContextAttributes(instanceId, contentBlockId, editContext, autoToolbar);

            var lineBreaks = addLineBreaks ? "\n" : "";

            return $"<{tag} class='{Constants.ClassToMarkContentBlock}' {contextAttribs}>{lineBreaks}"  +
                   $"{content}" +
                   $"{lineBreaks}</{tag}>";
        }


        public string ContextAttributes(int instanceId, int contentBlockId, bool includeEditInfos, bool autoToolbar)
        {
            var contextAttribs = "";
            if (instanceId != 0) contextAttribs += $" data-cb-instance='{instanceId}'";

            if (contentBlockId != 0) contextAttribs += $" data-cb-id='{contentBlockId}'";

            // optionally add editing infos
            if (includeEditInfos) contextAttribs += SexyContent.Html.Build.Attribute("data-edit-context", UiContextInfos(autoToolbar));
            return contextAttribs;
        }

        /// <summary>
        /// Return true if the URL is a debug URL
        /// </summary>
        private static bool IsDebugUrl(HttpRequest request) => string.IsNullOrEmpty(request.QueryString["debug"]);



        public void RegisterClientDependencies(Page page, bool readJs, bool editJs, bool editCss)
        {
            Log.Add("will auto-register client dependencies (js/css");
            var root = "~/desktopmodules/tosic_sexycontent/";
            root = page.ResolveUrl(root);
            //var breakCache = "?sxcver=" + Settings.Version;
            var ext = IsDebugUrl(page.Request) ? ".min.js" : ".js";// + breakCache;
            var ver = Settings.Version.ToString();

            // add edit-mode CSS
            if (editCss) RegisterCss(page, root + "dist/inpage/inpage.min.css");

            // add read-js
            if (readJs || editJs)
            {
                RegisterJs(page, ver, root + "js/2sxc.api" + ext);
                JavaScript.RequestRegistration(CommonJs.jQuery);
                // 2020-01-06 2sxc 10.25 - moved to here, might be a breaking change!
                ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            }

            // add edit-js (commands, manage, etc.)
            if (editJs) RegisterJs(page, ver, root + "dist/inpage/inpage.min.js");
        }

        #region add scripts / css with bypassing the official ClientResourceManager

        private static void RegisterJs(Page page, string version, string path)
        {
            var url = $"{path}{(path.IndexOf('?') > 0 ? '&' : '?')}v={version}";
            page.ClientScript.RegisterClientScriptInclude(typeof(Page), path, url);
        }

        private static void RegisterCss(Page page, string path) 
            => ClientResourceManager.RegisterStyleSheet(page, path);

        #endregion

        // new
        public string UiContextInfos(bool autoToolbars)
            => JsonConvert.SerializeObject(new ClientInfosAll(_applicationRoot, _portalSettings, _moduleInfo, _cmsInstance, _userInfo,
                _cmsInstance.Block.ZoneId // 2019-11-09, Id not nullable any more // ?? 0
                , _cmsInstance.Block.ContentGroupExists, autoToolbars, Log));



        public string DesignErrorMessage(Exception ex, bool addToEventLog, string visitorAlternateError, bool addMinimalWrapper, bool encodeMessage)
        {
            var intro = "Error";
            var msg = intro + ": " + ex;
            if (addToEventLog)
                Exceptions.LogException(ex);

            if (!_userInfo.IsSuperUser)
                msg = visitorAlternateError ?? "error showing content";

            if (encodeMessage)
                msg = HttpUtility.HtmlEncode(msg);

            // add dnn-error-div-wrapper
            msg = "<div class='dnnFormMessage dnnFormWarning'>" + msg + "</div>";

            // add another, minimal id-wrapper for those cases where the rendering-wrapper is missing
            if (addMinimalWrapper)
                msg = WrapInContext(msg, instanceId: _moduleInfo.Id, contentBlockId: _moduleInfo.Id);

            return msg;
        }

    }

}