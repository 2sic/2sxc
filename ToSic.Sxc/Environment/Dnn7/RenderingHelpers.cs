using System;
using System.Web;
using System.Web.UI;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Client.ClientResourceManagement;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;

// ReSharper disable InconsistentNaming

namespace ToSic.SexyContent.Environment.Dnn7
{
    internal class RenderingHelpers: HasLog
    {
        private readonly SxcInstance _sxcInstance;
        private readonly PortalSettings _portalSettings;
        private readonly UserInfo _userInfo;
        private readonly string _applicationRoot;
        private readonly IInstanceInfo _moduleInfo;

        internal RenderingHelpers(SxcInstance sxc, Log parentLog): base("DN.Render", parentLog)
        {
            string appRoot = VirtualPathUtility.ToAbsolute("~/");
            _moduleInfo = sxc?.InstanceInfo;
            _sxcInstance = sxc;
            _portalSettings = PortalSettings.Current;

            _userInfo = PortalSettings.Current.UserInfo;
            _applicationRoot = appRoot;

        }

        /// <summary>
        /// Return true if the URL is a debug URL
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static bool IsDebugUrl(HttpRequest request) => string.IsNullOrEmpty(request.QueryString["debug"]);

        internal void RegisterClientDependencies(Page page)
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

        private void RegisterJs(Page page, string version, string path)
        {
            var url = $"{path}{(path.IndexOf('?') > 0 ? '&' : '?')}v={version}";
            page.ClientScript.RegisterClientScriptInclude(typeof(Page), path, url);
        }

        private void RegisterCss(Page page, string path) => ClientResourceManager.RegisterStyleSheet(page, path);

        #endregion

        // new
        public ClientInfosAll GetClientInfosAll()
            => new ClientInfosAll(_applicationRoot, _portalSettings, _moduleInfo, _sxcInstance, _userInfo,
                _sxcInstance.ZoneId ?? 0, _sxcInstance.ContentBlock.ContentGroupExists, Log);

        public string DesignErrorMessage(Exception ex, bool addToEventLog, string visitorAlternateError, bool addMinimalWrapper, bool encodeMessage)
        {
            var intro = "Error"; // LocalizeString("TemplateError.Text") - todo i18n
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
                msg = "<div class='sc-content-block' data-cb-instance='" + _moduleInfo.Id + "' data-cb-id='" + _moduleInfo.Id + "'>" + msg + "</div>";

            return msg;
        }
    }

}