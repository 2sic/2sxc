using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.Oqt.Shared.Run;

namespace ToSic.Sxc.Oqt.Server.Page
{
    public class OqtAssetsAndHeaders: HasLog, IOqtAssetsAndHeader
    {
        #region Constructor and DI

        public OqtAssetsAndHeaders(IAntiforgery antiForgery, IHttpContextAccessor httpContextAccessor) : base("Oqt.AssHdr")
        {
            _antiForgery = antiForgery;
            _httpContextAccessor = httpContextAccessor;
        }
        private readonly IAntiforgery _antiForgery;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public void Init(SxcOqtane parent)
        {
            Parent = parent;
            BlockBuilder = parent?.Block?.BlockBuilder as BlockBuilder;
        }

        protected SxcOqtane Parent;
        protected BlockBuilder BlockBuilder;

        #endregion

        public bool AddContextMeta => AddJsCore || AddJsEdit;

        private bool AddJsCore => BlockBuilder?.UiAddJsApi ?? false;
        //public bool AddJsAdvanced = true;
        private bool AddJsEdit => BlockBuilder?.UiAddEditApi ?? false;
        private bool AddCssEdit => BlockBuilder?.UiAddEditUi ?? false;

        public IEnumerable<string> Scripts()
        {
            var list = new List<string>();
            if (AddJsCore) list.Add($"{OqtConstants.UiRoot}/{InpageCms.CoreJs}");
            if (AddJsEdit) list.Add($"{OqtConstants.UiRoot}/{InpageCms.EditJs}");
            return list;
        }

        public IEnumerable<string> Styles()
        {
            if (!AddCssEdit) return Array.Empty<string>();
            var list = new List<string>  { $"{OqtConstants.UiRoot}/{InpageCms.EditCss}" };
            return list;

        }

        public string ContextMetaContents()
        {
            var wrapLog = Log.Call<string>();

            var pageId = Parent?.Page.PageId ?? -1;
            var siteRoot = OqtConstants.SiteRoot;
            var idPart = Parent?.Site.SiteId.ToString();
            idPart = idPart == null ? WipConstants.WebApiPrefixFor1 : idPart + "/";
            var apiRoot = siteRoot + idPart + WebApiConstants.WebApiRoot + "/";
            var result = InpageCms.JsApiJson(pageId, siteRoot, apiRoot, AntiForgeryToken(), OqtConstants.UiRoot + "/");
            return wrapLog("ok", result);
        }

        public string ContextMetaName => InpageCms.MetaName;

        private string AntiForgeryToken() 
            => _antiForgery.GetAndStoreTokens(_httpContextAccessor.HttpContext).RequestToken;
    }
}
