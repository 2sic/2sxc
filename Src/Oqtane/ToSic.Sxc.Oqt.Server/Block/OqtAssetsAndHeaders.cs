using Oqtane.Shared;
using System;
using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.PageFeatures;

namespace ToSic.Sxc.Oqt.Server.Block
{
    [PrivateApi]
    public partial class OqtAssetsAndHeaders : HasLog
    {
        #region Constructor and DI

        public OqtAssetsAndHeaders(SiteState siteState, IClientDependencyOptimizer oqtClientDependencyOptimizer) : base($"{OqtConstants.OqtLogPrefix}.AssHdr")
        {
            _siteState = siteState;
            _oqtClientDependencyOptimizer = oqtClientDependencyOptimizer.Init(Log);
        }

        private readonly SiteState _siteState;
        private readonly IClientDependencyOptimizer _oqtClientDependencyOptimizer;
        
        public void Init(OqtSxcViewBuilder parent, RenderResult renderResult)
        {
            Parent = parent;
            RenderResult = renderResult;
        }

        protected OqtSxcViewBuilder Parent;
        protected RenderResult RenderResult;

        #endregion


        private bool AddJsCore => Features.Contains(BuiltInFeatures.Core);
        private bool AddJsEdit => Features.Contains(BuiltInFeatures.EditApi);
        private bool AddCssEdit => Features.Contains(BuiltInFeatures.EditUi);


        /// <summary>
        /// The JavaScripts needed
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Scripts()
        {
            var list = new List<string>();

            // v12.03, Oqtane 2.2 with Bootstrap 5 do not includes jQuery any more
            // as Oqtane 2.1 with Bootstrap 4
            if (Features.Contains(BuiltInFeatures.JQuery)) // #2492 disabled:   || AddJsEdit)
                list.Add("//code.jquery.com/jquery-3.5.1.min.js");

            if (AddJsCore) 
                    list.Add($"{OqtConstants.UiRoot}/{InpageCms.CoreJs}");

            if (AddJsEdit)
                list.Add($"{OqtConstants.UiRoot}/{InpageCms.EditJs}");

            // New in 12.02
            if (Features.Contains(BuiltInFeatures.TurnOn))
                list.Add($"{OqtConstants.UiRoot}/{InpageCms.TurnOnJs}");

            return list;
        }

        /// <summary>
        /// The styles to add
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Styles()
        {
            if (!AddCssEdit) return Array.Empty<string>();
            return new List<string> { $"{OqtConstants.UiRoot}/{InpageCms.EditCss}" };
        }

        [PrivateApi]
        public static string GetSiteRoot(SiteState siteState)
            => siteState?.Alias?.Name == null ? OqtConstants.SiteRoot : new Uri($"http://{siteState.Alias.Name}/").AbsolutePath.SuffixSlash();

        internal List<IPageFeature> Features => _features ??= RenderResult.Features ?? new List<IPageFeature>();
        private List<IPageFeature> _features;

    }
}
