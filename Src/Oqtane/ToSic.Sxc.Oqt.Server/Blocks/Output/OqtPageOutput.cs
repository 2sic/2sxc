using System;
using System.Collections.Generic;
using Oqtane.Shared;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Web.PageFeatures;

namespace ToSic.Sxc.Oqt.Server.Blocks.Output
{
    [PrivateApi]
    public partial class OqtPageOutput : HasLog
    {
        #region Constructor and DI

        public OqtPageOutput(SiteState siteState, IBlockResourceExtractor oqtBlockResourceExtractor) : base($"{OqtConstants.OqtLogPrefix}.AssHdr")
        {
            _siteState = siteState;
            _oqtBlockResourceExtractor = oqtBlockResourceExtractor.Init(Log);
        }

        private readonly SiteState _siteState;
        private readonly IBlockResourceExtractor _oqtBlockResourceExtractor;
        
        public void Init(OqtSxcViewBuilder parent, IRenderResult renderResult)
        {
            Parent = parent;
            RenderResult = renderResult;
        }

        protected OqtSxcViewBuilder Parent;
        protected IRenderResult RenderResult;

        #endregion


        private bool AddJsCore => Features.Contains(BuiltInFeatures.JsCore);
        private bool AddJsEdit => Features.Contains(BuiltInFeatures.JsCms);
        private bool AddCssEdit => Features.Contains(BuiltInFeatures.Toolbars);


        /// <summary>
        /// The JavaScripts needed
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Scripts()
        {
            var list = new List<string>();

            // v12.03, Oqtane 2.2 with Bootstrap 5 do not includes jQuery any more
            // as Oqtane 2.1 with Bootstrap 4
            if (Features.Contains(BuiltInFeatures.JQuery)) 
                list.Add("//code.jquery.com/jquery-3.5.1.min.js");

            if (AddJsCore) list.Add($"{OqtConstants.UiRoot}/{InpageCms.CoreJs}");

            if (AddJsEdit) list.Add($"{OqtConstants.UiRoot}/{InpageCms.EditJs}");

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

        internal IList<IPageFeature> Features => _features ??= RenderResult.Features ?? new List<IPageFeature>();
        private IList<IPageFeature> _features;

    }
}
