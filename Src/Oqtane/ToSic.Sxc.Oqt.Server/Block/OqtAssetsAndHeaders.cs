using System;
using System.Collections.Generic;
//using Microsoft.AspNetCore.Antiforgery;
//using Microsoft.AspNetCore.Http;
using Oqtane.Shared;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Block
{
    [PrivateApi]
    public class OqtAssetsAndHeaders: HasLog, IOqtAssetsAndHeader
    {
        #region Constructor and DI

        public OqtAssetsAndHeaders(/*IAntiforgery antiForgery, IHttpContextAccessor httpContextAccessor,*/ SiteState siteState) : base($"{OqtConstants.OqtLogPrefix}.AssHdr")
        {
            //_antiForgery = antiForgery;
            //_httpContextAccessor = httpContextAccessor;
            _siteState = siteState;
        }
        //private readonly IAntiforgery _antiForgery;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SiteState _siteState;


        public void Init(OqtSxcViewBuilder parent)
        {
            Parent = parent;
            BlockBuilder = parent?.Block?.BlockBuilder as BlockBuilder;
        }

        protected OqtSxcViewBuilder Parent;
        protected BlockBuilder BlockBuilder;

        #endregion

        public bool AddContextMeta => AddJsCore || AddJsEdit;

        private bool AddJsCore => BlockBuilder?.UiAddJsApi ?? false;
        private bool AddJsEdit => BlockBuilder?.UiAddEditApi ?? false;
        private bool AddCssEdit => BlockBuilder?.UiAddEditUi ?? false;

        public IEnumerable<string> Scripts()
        {
            var list = new List<string>();
            if (AddJsCore) list.Add($"{OqtConstants.UiRoot}/{InpageCms.CoreJs}");
            if (AddJsEdit) list.Add($"{OqtConstants.UiRoot}/{InpageCms.EditJs}");
            if(BlockBuilder.NamedScriptsWIP?.Contains(BlockBuilder.JsTurnOn) ?? false)
                list.Add($"{OqtConstants.UiRoot}/{InpageCms.TurnOnJs}");
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
            var siteRoot = GetSiteRoot(_siteState);
            var apiRoot = siteRoot + WebApiConstants.ApiRoot + "/";
            var appApiRoot = siteRoot; // without "app/" because the UI will add that later on
            var result = InpageCms.JsApiJson(
                PlatformType.Oqtane.ToString(),
                pageId, 
                siteRoot, 
                apiRoot, 
                appApiRoot, 
                AntiForgeryToken(), 
                OqtConstants.UiRoot + "/");
            return wrapLog("ok", result);
        }

        public string ContextMetaName => InpageCms.MetaName;

        private string AntiForgeryToken() => ""; // 2021-05-11 the Token given by IAntiforgery is always wrong, so better keep it empty
                                                 // Reason is probably that at this moment the render-request is running in another HttpContext
                                                 // => _antiForgery.GetAndStoreTokens(_httpContextAccessor.HttpContext).RequestToken;

        [PrivateApi]
        public static string GetSiteRoot(SiteState siteState)
            => siteState?.Alias?.Name == null ? OqtConstants.SiteRoot : new Uri($"http://{siteState.Alias.Name}/").AbsolutePath.SuffixSlash();
    }
}
