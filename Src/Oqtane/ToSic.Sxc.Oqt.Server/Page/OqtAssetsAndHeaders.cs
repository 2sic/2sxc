using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using ToSic.Eav.Logging;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.Oqt.Shared.Run;

namespace ToSic.Sxc.Oqt.Server.Page
{
    public class OqtAssetsAndHeaders: HasLog, IOqtAssetsAndHeader
    {
        private readonly IAntiforgery _antiForgery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #region Constructor and DI

        public OqtAssetsAndHeaders(IAntiforgery antiForgery, IHttpContextAccessor httpContextAccessor) : base("Oqt.AssHdr")
        {
            _antiForgery = antiForgery;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        public bool AddContextMeta => true;

        public bool AddJsCore = true;
        //public bool AddJsAdvanced = true;
        public bool AddCmsJs = true;
        public bool AddCmsCss = true;

        public IEnumerable<string> Scripts()
        {
            var list = new List<string>();
            if (AddJsCore) list.Add($"{OqtConstants.UiRoot}/{InpageCms.CoreJs}");
            if (AddCmsJs) list.Add($"{OqtConstants.UiRoot}/{InpageCms.EditJs}");
            return list;
        }

        public IEnumerable<string> Styles()
        {
            if (!AddCmsCss) return Array.Empty<string>();
            var list = new List<string>  { $"{OqtConstants.UiRoot}/{InpageCms.EditCss}" };
            return list;
        }

        // TODO: #Oqtane
        public string ContextMetaContents()
        {
            var wrapLog = Log.Call<string>();

            var pageId = 0;
            var siteRoot = OqtConstants.SiteRoot;
            var apiRoot = siteRoot + WipConstants.WebApiPrefixFor1 + WebApiConstants.WebApiRoot + "/";
            var result = InpageCms.JsApiJson(pageId, siteRoot, apiRoot, AntiForgeryToken(), OqtConstants.UiRoot + "/");
            return wrapLog("ok", result);
        }

        public string ContextMetaName => InpageCms.MetaName;

        private string AntiForgeryToken() 
            => _antiForgery.GetAndStoreTokens(_httpContextAccessor.HttpContext).RequestToken;
    }
}
