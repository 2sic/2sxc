using System;
using System.Collections.Generic;
using System.Linq;
using Oqtane.Shared;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.PageFeatures;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Oqt.Server.Block
{
    [PrivateApi]
    public partial class OqtAssetsAndHeaders : HasLog
    {
        #region Constructor and DI

        public OqtAssetsAndHeaders(SiteState siteState, IClientDependencyOptimizer oqtClientDependencyOptimizer) : base($"{OqtConstants.OqtLogPrefix}.AssHdr")
        {
            OqtClientDependencyOptimizer = oqtClientDependencyOptimizer.Init(Log);
            _siteState = siteState;
        }

        public IClientDependencyOptimizer OqtClientDependencyOptimizer { get; }
        private readonly SiteState _siteState;

        public void Init(OqtSxcViewBuilder parent, RenderResultWIP renderResult)
        {
            Parent = parent;
            RenderResult = renderResult;
            BlockBuilder = parent?.Block?.BlockBuilder as BlockBuilder;
        }

        protected OqtSxcViewBuilder Parent;
        protected RenderResultWIP RenderResult;
        protected BlockBuilder BlockBuilder;

        #endregion


        private bool AddJsCore => Features.Contains(BuiltInFeatures.Core); // || (BlockBuilder?.UiAddJsApi ?? AddJsEdit); //BlockBuilder?.UiAddJsApi ?? false;
        private bool AddJsEdit => Features.Contains(BuiltInFeatures.EditApi); // || (BlockBuilder?.UiAddEditApi ?? false);  // BlockBuilder?.UiAddEditApi ?? false;
        private bool AddCssEdit => Features.Contains(BuiltInFeatures.EditUi); // || (BlockBuilder?.UiAddEditUi ?? false);  // BlockBuilder?.UiAddEditUi ?? false;
        

        /// <summary>
        /// The JavaScript and Style assets
        /// from razor template and manual features
        /// </summary>
        /// <returns></returns>
        public List<SxcResource> GetSxcResources()
        {
            // assets from razor template
            var resources = SxcResourcesBuilder(RenderResult.Assets);
            // assets from manual features
            resources.AddRange(SxcResourcesBuilder(GetAssetsFromManualFeatures()));
            return resources;
        }

        private static List<SxcResource> SxcResourcesBuilder(List<ClientAssetInfo> assets)
        {
            var resources = assets.Select(a => new SxcResource
            {
                ResourceType = a.IsJs ? ResourceType.Script : ResourceType.Stylesheet,
                Url = a.Url,
                IsExternal = a.IsExternal,
                Content = a.Content,
                UniqueId = a.Id
            }).ToList();
            return resources;
        }

        private List<ClientAssetInfo> GetAssetsFromManualFeatures()
        {
            var assets = new List<ClientAssetInfo>();
            foreach (var manualFeature in ManualFeatures)
            {
                // process manual features to get assets
                OqtClientDependencyOptimizer.Process(manualFeature.Html);
                assets.AddRange(OqtClientDependencyOptimizer.Assets);
            }
            return assets;
        }

        /// <summary>
        /// The JavaScripts needed
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Scripts()
        {
            var list = new List<string>();


            // v12.03, Oqtane 2.2 with Bootstrap 5 do not includes jQuery any more
            // as Oqtane 2.1 with Bootstrap 4
            if (Features.Contains(BuiltInFeatures.JQuery) || AddJsEdit)
                list.Add($"//code.jquery.com/jquery-3.5.1.min.js");

            if (AddJsCore)
                    list.Add($"{OqtConstants.UiRoot}/{InpageCms.CoreJs}");

            if (AddJsEdit)
                list.Add($"{OqtConstants.UiRoot}/{InpageCms.EditJs}");

            //if(BlockBuilder.NamedScriptsWIP?.Contains(BlockBuilder.JsTurnOn) ?? false)
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
            var list = new List<string> { $"{OqtConstants.UiRoot}/{InpageCms.EditCss}" };

            // Manual styles


            return list;
        }

        [PrivateApi]
        public static string GetSiteRoot(SiteState siteState)
            => siteState?.Alias?.Name == null ? OqtConstants.SiteRoot : new Uri($"http://{siteState.Alias.Name}/").AbsolutePath.SuffixSlash();

        internal List<IPageFeature> Features => _features ??= RenderResult.Features ?? new List<IPageFeature>();
        private List<IPageFeature> _features;

        internal IList<IPageFeature> ManualFeatures => _manualFeatures ??= RenderResult.ManualChanges ?? new List<IPageFeature>();
        private IList<IPageFeature> _manualFeatures;
    }
}
