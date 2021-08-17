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

            // TODO: This is quick fix, because Features are empty.
            // v12.03, Oqtane 2.2 with Bootstrap 5 do not includes jQuery any more
            // as Oqtane 2.1 with Bootstrap 4
            // https://code.jquery.com/jquery-3.5.1.slim.min.js
            // "slim" version excludes ajax and effects modules
            list.Add($"//code.jquery.com/jquery-3.5.1.slim.min.js");

            if (AddJsCore) list.Add($"{OqtConstants.UiRoot}/{InpageCms.CoreJs}");
            if (AddJsEdit) list.Add($"{OqtConstants.UiRoot}/{InpageCms.EditJs}");
            //if(BlockBuilder.NamedScriptsWIP?.Contains(BlockBuilder.JsTurnOn) ?? false)
            // New in 12.02
            if(Features.Contains(BuiltInFeatures.TurnOn))
                list.Add($"{OqtConstants.UiRoot}/{InpageCms.TurnOnJs}");

            if (Features.Contains(BuiltInFeatures.JQuery))
            {
                // v12.03, Oqtane 2.2 with Bootstrap 5 do not includes jQuery any more
                // as Oqtane 2.1 with Bootstrap 4
                // https://code.jquery.com/jquery-3.5.1.slim.min.js
                // "slim" version excludes ajax and effects modules
                list.Add($"//code.jquery.com/jquery-3.5.1.slim.min.js");
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
