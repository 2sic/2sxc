using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Client.ClientResourceManagement;
using ToSic.SexyContent.Interfaces;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.Environment.Dnn7
{
    internal class RenderingHelpers
    {
        private readonly SxcInstance _sxcInstance;
        private readonly PortalSettings _portalSettings;
        private readonly UserInfo _userInfo;
        private readonly string _applicationRoot;
        private readonly ModuleInfo _moduleInfo;

        internal RenderingHelpers(SxcInstance sxc)
        {
            string appRoot = VirtualPathUtility.ToAbsolute("~/");
            _moduleInfo = sxc?.ModuleInfo;// mi;// mic.Configuration;
            _sxcInstance = sxc;
            _portalSettings = PortalSettings.Current; // mic.PortalSettings;// PortalSettings.Current;

            _userInfo = PortalSettings.Current.UserInfo;
            _applicationRoot = appRoot;

        }

        /// <summary>
        /// Return true if the URL is a debug URL
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static bool IsDebugUrl(HttpRequest request)
        {
            return string.IsNullOrEmpty(request.QueryString["debug"]);
        }


        internal void RegisterClientDependencies(Page page)
        {
            var root = "~/desktopmodules/tosic_sexycontent/";
            root = page.ResolveUrl(root);
            var breakCache = "?sxcver=" + Settings.Version;
            var ext = (IsDebugUrl(page.Request) ? ".min.js" : ".js" + breakCache);
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

        private void RegisterCss(Page page, string path)
        {
            ClientResourceManager.RegisterStyleSheet(page, path);
        }


        #endregion

        // new
        public ClientInfosAll GetClientInfosAll()
        {
            return new ClientInfosAll(_applicationRoot, _portalSettings, _moduleInfo, _sxcInstance, _userInfo,
                    _sxcInstance.ZoneId ?? 0, _sxcInstance.ContentBlock.ContentGroupExists);
        }

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
                msg = "<div class='sc-content-block' data-cb-instance='" + _moduleInfo.ModuleID + "' data-cb-id='" + _moduleInfo.ModuleID + "'>" + msg + "</div>";

            return msg;

        }

    }




    #region ClientInfos Objects to generate the json-attribute
    public class ClientInfosAll
    {
        public ClientInfosEnvironment Environment;
        public ClientInfosUser User;
        public ClientInfosLanguages Language;
        public ClientInfoContentBlock ContentBlock; // todo: still not sure if these should be separate...
        public ClientInfoContentGroup ContentGroup;
        // ReSharper disable once InconsistentNaming
        public ClientInfosError error;

        public ClientInfosAll(string systemRootUrl, PortalSettings ps, ModuleInfo mic, SxcInstance sxc, UserInfo uinfo, int zoneId, bool isCreated)
        {
            Environment = new ClientInfosEnvironment(systemRootUrl, ps, mic, sxc);
            Language = new ClientInfosLanguages(ps, zoneId);
            User = new ClientInfosUser(uinfo);

            ContentBlock = new ClientInfoContentBlock(sxc.ContentBlock, null, 0);
            ContentGroup = new ClientInfoContentGroup(sxc, isCreated);
            error = new ClientInfosError(sxc.ContentBlock);
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ClientInfosError
    {
        public string type;

        internal ClientInfosError(IContentBlock cb)
        {
            if (cb.DataIsMissing)
            {
                type = "DataIsMissing";
            }
        }
    }

    public class ClientInfosEnvironment
    {
        public int WebsiteId;       // aka PortalId
        public string WebsiteUrl;
        public int PageId;          // aka TabId
        public string PageUrl;

        public int InstanceId;      // aka ModuleId

        public string SxcVersion;

        public string SxcRootUrl;

        public bool IsEditable;

        public ClientInfosEnvironment(string systemRootUrl, PortalSettings ps, ModuleInfo mic, SxcInstance sxc)
        {
            WebsiteId = ps.PortalId;
            WebsiteUrl = "//" + ps.PortalAlias.HTTPAlias + "/";

            PageId = mic.TabID;
            PageUrl = ps.ActiveTab.FullUrl;

            InstanceId = mic.ModuleID;

            SxcVersion = Settings.Version.ToString();

            SxcRootUrl = systemRootUrl;

            IsEditable = sxc?.Environment?.Permissions.UserMayEditContent ?? false;
        }
    }

    public class ClientInfosUser
    {
        public bool CanDesign;
        public bool CanDevelop;

        public ClientInfosUser(UserInfo uinfo)
        {
            CanDesign = SecurityHelpers.IsInSexyContentDesignersGroup(uinfo);
            CanDevelop = uinfo.IsSuperUser;
        }
    }

    public class ClientInfosLanguages
    {
        public string Current;
        public string Primary;
        public IEnumerable<ClientInfoLanguage> All;

        public ClientInfosLanguages(PortalSettings ps, int zoneId)
        {
            Current = ps.CultureCode.ToLower();
            Primary = ps.DefaultLanguage.ToLower();
            All = ZoneHelpers.GetCulturesWithActiveState(ps.PortalId, zoneId)
                    .Where(c => c.Active)
                    .Select(c => new ClientInfoLanguage() { key = c.Code.ToLower(), name = c.Text });
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ClientInfoLanguage
    {
        // key and name must be lowercase, has side effects in EAV
        public string key;
        public string name;
    }

    public class ClientInfoContentBlock //: ClientInfoEntity
    {
        public bool ShowTemplatePicker;
        public bool IsEntity;
        public int Id;
        public string ParentFieldName;
        public int ParentFieldSortOrder;
        // public bool DataIsMissing;

        internal ClientInfoContentBlock(IContentBlock contentBlock, string parentFieldName, int indexInField)
        {
            ShowTemplatePicker = contentBlock.ShowTemplateChooser;
            IsEntity = contentBlock.ParentIsEntity;
            Id = contentBlock.ContentBlockId;
            ParentFieldName = parentFieldName;
            ParentFieldSortOrder = indexInField;
            // DataIsMissing = contentBlock.DataIsMissing;
        }
    };

    public class ClientInfoContentGroup: ClientInfoEntity
    {
        public bool IsCreated;
        public bool IsList;
        public int TemplateId;
        public string ContentTypeName;
        public string AppUrl;
        public bool IsContent;
        public bool HasContent;

        public ClientInfoContentGroup(SxcInstance sxc, bool isCreated)
        {
            IsCreated = isCreated;
            IsContent = sxc.IsContentApp;

            Id = sxc.ContentGroup?.ContentGroupId ?? 0;
            Guid = sxc.ContentGroup?.ContentGroupGuid ?? Guid.Empty;
            AppId = sxc.AppId ?? 0;
            AppUrl = sxc.App?.Path ?? "" + "/" ;
            HasContent = sxc.Template != null && (sxc.ContentGroup?.Exists ?? false);

            ZoneId = sxc.ZoneId ?? 0;
            TemplateId = sxc.Template?.TemplateId ?? 0;     // todo: check if the 0 (previously null) causes problems
            ContentTypeName = sxc.Template?.ContentTypeStaticName ?? "";
            IsList = sxc.ContentGroup?.Template?.UseForList ?? false;//  isCreated && ((sxc.ContentGroup?.Content?.Count ?? 0) > 1);
        }
    }

    public abstract class ClientInfoEntity
    {
        public int ZoneId;  // the zone of the content-block
        public int AppId;   // the zone of the content-block
        public Guid Guid;   // the entity-guid of the content-block
        public int Id;      // the entity-id of the content-block
    }

    #endregion
}