using System;
using System.Collections.Generic;
using Oqtane.Shared;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.PageFeatures;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Oqt.Server.Block
{
    [PrivateApi]
    public partial class OqtAssetsAndHeaders : HasLog
    {
        #region Constructor and DI

        public OqtAssetsAndHeaders(SiteState siteState, /*IPageService pageService,*/ PageServiceShared pageServiceShared) : base($"{OqtConstants.OqtLogPrefix}.AssHdr")
        {
            _siteState = siteState;
            //PageService = pageService;
            PageServiceShared = pageServiceShared;
        }
        private readonly SiteState _siteState;
        //private IPageService PageService { get; }
        public PageServiceShared PageServiceShared { get; }

        public void Init(OqtSxcViewBuilder parent)
        {
            Parent = parent;
            BlockBuilder = parent?.Block?.BlockBuilder as BlockBuilder;

            // Temp added here. It should be automatic later.
            //if (AddJsCore) PageServiceShared.Activate(BuiltInFeatures.Core.Key);
            //if (AddJsEdit) PageServiceShared.Activate(BuiltInFeatures.EditApi.Key);
        }

        protected OqtSxcViewBuilder Parent;
        protected BlockBuilder BlockBuilder;

        #endregion


        private bool AddJsCore => Features.Contains(BuiltInFeatures.Core) || (BlockBuilder?.UiAddJsApi ?? AddJsEdit); //BlockBuilder?.UiAddJsApi ?? false;
        private bool AddJsEdit => Features.Contains(BuiltInFeatures.EditApi) || (BlockBuilder?.UiAddEditApi ?? false);  // BlockBuilder?.UiAddEditApi ?? false;
        private bool AddCssEdit => Features.Contains(BuiltInFeatures.EditUi) || (BlockBuilder?.UiAddEditUi ?? false);  // BlockBuilder?.UiAddEditUi ?? false;

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
            return list;
        }

        [PrivateApi]
        public static string GetSiteRoot(SiteState siteState)
            => siteState?.Alias?.Name == null ? OqtConstants.SiteRoot : new Uri($"http://{siteState.Alias.Name}/").AbsolutePath.SuffixSlash();

        internal List<IPageFeature> Features => _features ??= /*PageService*/PageServiceShared.Features.GetWithDependentsAndFlush(Log);
        private List<IPageFeature> _features;

        /// <summary>
        /// The JavaScripts needed
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetPageHeadUpdates()
        {
            var headUpdates = new List<string>();

            // TODO: STV, remove dummy test
            var dummyTest = "<link data-enableoptimizations=\"bottom\" rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/fancybox/3.5.7/jquery.fancybox.min.css\" integrity=\"sha512-H9jrZiiopUdsLpg94A333EfumgUBpO9MdbxStdeITo+KEIMaNfHNvwyjjDJb+ERPaRS6DpyRlKbvPUasNItRyw==\" crossorigin=\"anonymous\" referrerpolicy=\"no-referrer\" /> <script defer async data-enableoptimizations=\"bottom\" src=\"https://cdnjs.cloudflare.com/ajax/libs/fancybox/3.5.7/jquery.fancybox.min.js\" integrity=\"sha512-uURl+ZXMBrF4AwGaWmEetzrd+J5/8NRkWAvJx5sbPSSuOb0bZLqf+tOzniObO00BjHa/dD7gub9oCGMLPQHtQA==\" crossorigin=\"anonymous\" referrerpolicy=\"no-referrer\"></script>";
            headUpdates.Add(dummyTest);

            // TODO: STV WIP...
            //_oqtClientDependencyOptimizer.Process(dummyTest);

            //var x = PageService
            foreach (var f in PageServiceShared.Features.ManualFeaturesGetNew())
            {
                headUpdates.Add(f.Html);
                // TODO: STV alternative - check OqtClientDependencyOptimizer.Process to extract js and script files
            }

            return headUpdates;
        }

    }
}
