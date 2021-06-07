using System;
using System.Collections.Generic;
using Oqtane.Shared;
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
    public partial class OqtAssetsAndHeaders: HasLog
    {
        #region Constructor and DI

        public OqtAssetsAndHeaders(SiteState siteState, IPageService pageService, IPageFeaturesManager pageFm) : base($"{OqtConstants.OqtLogPrefix}.AssHdr")
        {
            _siteState = siteState;
            PageService = pageService;
            _pageFm = pageFm;
        }
        private readonly SiteState _siteState;
        private IPageService PageService { get; }
        private readonly IPageFeaturesManager _pageFm;


        public void Init(OqtSxcViewBuilder parent)
        {
            Parent = parent;
            BlockBuilder = parent?.Block?.BlockBuilder as BlockBuilder;
        }

        protected OqtSxcViewBuilder Parent;
        protected BlockBuilder BlockBuilder;

        #endregion


        private bool AddJsCore => BlockBuilder?.UiAddJsApi ?? false;
        private bool AddJsEdit => BlockBuilder?.UiAddEditApi ?? false;
        private bool AddCssEdit => BlockBuilder?.UiAddEditUi ?? false;

        /// <summary>
        /// The JavaScripts needed
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Scripts()
        {
            var list = new List<string>();
            if (AddJsCore) list.Add($"{OqtConstants.UiRoot}/{InpageCms.CoreJs}");
            if (AddJsEdit) list.Add($"{OqtConstants.UiRoot}/{InpageCms.EditJs}");
            //if(BlockBuilder.NamedScriptsWIP?.Contains(BlockBuilder.JsTurnOn) ?? false)
            // New in 12.02 - TODO: VERIFY IT WORK @SPM (I haven't verified this yet)
            if(Features.Contains(BuiltInFeatures.TurnOn))
                list.Add($"{OqtConstants.UiRoot}/{InpageCms.TurnOnJs}");

            if (Features.Contains(BuiltInFeatures.JQuery))
            {
                // v12.02 don't do anything yet, Oqtane always includes jQuery as of now
                // https://code.jquery.com/jquery-3.5.1.slim.min.js
                // "slim" version excludes ajax and effects modules
                // list.Add($"{OqtConstants.UiRoot}/{InpageCms.TurnOnJs}");
            }

            if (Features.Contains(BuiltInFeatures.JQueryUi))
            {
                // TODO: SPM - please find a proper place to add this from a CDN in a matching version as is in Oqtane
                // /*! jQuery UI - v1.12.1 - 2016-09-14
                list.Add($"https://code.jquery.com/ui/1.12.1/jquery-ui.min.js");
            }


            return list;
        }

        /// <summary>
        /// The styles to add
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Styles()
        {
            if (!AddCssEdit) return Array.Empty<string>();
            var list = new List<string>  { $"{OqtConstants.UiRoot}/{InpageCms.EditCss}" };
            return list;
        }

        [PrivateApi]
        public static string GetSiteRoot(SiteState siteState)
            => siteState?.Alias?.Name == null ? OqtConstants.SiteRoot : new Uri($"http://{siteState.Alias.Name}/").AbsolutePath.SuffixSlash();

        internal List<IPageFeature> Features => _features ??= PageService.Features.GetWithDependentsAndFlush(Log);
        private List<IPageFeature> _features;



    }
}
