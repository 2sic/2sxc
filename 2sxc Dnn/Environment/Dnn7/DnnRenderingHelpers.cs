using System;
using System.Web;
using System.Web.UI;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Client.ClientResourceManagement;
using Newtonsoft.Json;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.Edit.InPageEditingSystem;
using ToSic.Sxc.Interfaces;

// ReSharper disable InconsistentNaming
namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnRenderingHelpers : IHasLog, IRenderingHelpers
    {
        private SxcInstance _sxcInstance;
        private PortalSettings _portalSettings;
        private UserInfo _userInfo;
        private string _applicationRoot;
        private IInstanceInfo _moduleInfo;

        public Log Log = new Log("Dnn.Render");

        // Blank constructor for IoC
        public DnnRenderingHelpers() { }

        public DnnRenderingHelpers(SxcInstance sxc, Log parentLog) => Init(sxc, parentLog);

        public IRenderingHelpers Init(SxcInstance sxc, Log parentLog)
        {
            LinkLog(parentLog);
            var appRoot = VirtualPathUtility.ToAbsolute("~/");
            _moduleInfo = sxc?.InstanceInfo;
            _sxcInstance = sxc;
            _portalSettings = PortalSettings.Current;

            _userInfo = PortalSettings.Current.UserInfo;
            _applicationRoot = appRoot;

            return this;
        }

        public string WrapInContext(string content, 
            IRenderingHelpers dontUseThis = null, 
            int instanceId = 0, 
            int contentBlockId = 0, 
            bool includeEditInfos = false,
            string moreAttribs = null, 
            string moreClasses = null)
        {
            var head = $"<div class='sc-content-block {moreClasses}'";
            if (instanceId != 0) head += $" data-cb-instance='{instanceId}'";

            if (contentBlockId != 0) head += $" data-cb-id='{contentBlockId}'";

            if (moreAttribs != null) head += $" {moreAttribs}";

            // optionally add editing infos
            if (includeEditInfos)
            {
                var editHelper = new InPageEditingHelper(includeEditInfos, Log);
                head += editHelper.Attribute("data-edit-context", GetClientInfosAll());
            }

            head += ">\n";

            return head + content + "</div>";
        }

        /// <summary>
        /// Return true if the URL is a debug URL
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static bool IsDebugUrl(HttpRequest request) => string.IsNullOrEmpty(request.QueryString["debug"]);



        public void RegisterClientDependencies(Page page)
        {
            Log.Add("will auto-register client dependencies (js/css");
            var root = "~/desktopmodules/tosic_sexycontent/";
            root = page.ResolveUrl(root);
            var breakCache = "?sxcver=" + Settings.Version;
            var ext = IsDebugUrl(page.Request) ? ".min.js" : ".js" + breakCache;
            var ver = Settings.Version.ToString();

            // add edit-mode CSS
            RegisterCss(page, root + "dist/inpage/inpage.min.css");

            RegisterJs(page, ver, root + "js/2sxc.api" + ext);
            RegisterJs(page, ver, root + "dist/inpage/inpage" + ext);
        }

        #region add scripts / css with bypassing the official ClientResourceManager

        private static void RegisterJs(Page page, string version, string path)
        {
            var url = $"{path}{(path.IndexOf('?') > 0 ? '&' : '?')}v={version}";
            page.ClientScript.RegisterClientScriptInclude(typeof(Page), path, url);
        }

        private static void RegisterCss(Page page, string path) => ClientResourceManager.RegisterStyleSheet(page, path);

        #endregion

        // new
        public string GetClientInfosAll()
            => JsonConvert.SerializeObject(new ClientInfosAll(_applicationRoot, _portalSettings, _moduleInfo, _sxcInstance, _userInfo,
                _sxcInstance.ZoneId ?? 0, _sxcInstance.ContentBlock.ContentGroupExists, Log));



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
                // msg = "<div class='sc-content-block' data-cb-instance='" + _moduleInfo.Id + "' data-cb-id='" + _moduleInfo.Id + "'>" + msg + "</div>";

            return msg;
        }

        public void LinkLog(Log parentLog) => Log.LinkTo(parentLog);
    }

}