using System.Linq;
using Oqtane.Models;
using Oqtane.Shared;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Oqt.Server.Installation;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Oqt.Server.Block
{
    [PrivateApi]
    public class OqtSxcViewBuilder : HasLog, ISxcOqtane
    {
        #region Constructor and DI

        public OqtSxcViewBuilder(OqtAssetsAndHeaders assetsAndHeaders, OqtState oqtState, IClientDependencyOptimizer oqtClientDependencyOptimizer) : base($"{OqtConstants.OqtLogPrefix}.Buildr")
        {

            _assetsAndHeaders = assetsAndHeaders;
            _oqtClientDependencyOptimizer = oqtClientDependencyOptimizer.Init(Log);
            _oqtState = oqtState.Init(Log);
            // add log to history!
            History.Add("oqt-view", Log);
        }

        private OqtAssetsAndHeaders AssetsAndHeaders => _assetsAndHeaders;
        private readonly OqtAssetsAndHeaders _assetsAndHeaders;
        private readonly IClientDependencyOptimizer _oqtClientDependencyOptimizer;
        private readonly OqtState _oqtState;

        #endregion

        #region Prepare

        /// <summary>
        /// Prepare must always be the first thing to be called - to ensure that afterwards both headers and html are known.
        /// </summary>
        public OqtViewResultsDto Prepare(Alias alias, Site site, Page page, Module module)
        {
            // Check for installation errors before even trying to build a view, and otherwise return this object if Refs are missing.
            if (RefsInstalledCheck.WarnIfRefsAreNotInstalled(out var oqtViewResultsDto)) return oqtViewResultsDto;

            Alias = alias;
            Site = site;
            Page = page;
            Module = module;

            Block = _oqtState.GetBlockOfModule(page.PageId, module);

            _assetsAndHeaders.Init(this);
            var generatedHtml = Block.BlockBuilder.Render() ;
            var resources = Block.BlockBuilder.Assets.Select(a => new SxcResource
            {
                ResourceType = a.IsJs ? ResourceType.Script : ResourceType.Stylesheet,
                Url = a.Url,
                IsExternal = a.IsExternal,
                Content = a.Content,
                UniqueId = a.Id
            }).ToList();

            var pageHeadUpdates = AssetsAndHeaders.GetPageHeadUpdates().ToList();
            foreach (var html in pageHeadUpdates)
            {

                _oqtClientDependencyOptimizer.Process(html);
            }

            return new OqtViewResultsDto
            {
                Html = generatedHtml,
                TemplateResources = resources,
                SxcContextMetaName = AssetsAndHeaders.AddContextMeta ? AssetsAndHeaders.ContextMetaName : null,
                SxcContextMetaContents = AssetsAndHeaders.AddContextMeta ? AssetsAndHeaders.ContextMetaContents(): null,
                SxcScripts = AssetsAndHeaders.Scripts().ToList(),
                SxcStyles = AssetsAndHeaders.Styles().ToList(),
                PageProperties = AssetsAndHeaders.GetPagePropertyChanges(),
                PageHeadUpdates = pageHeadUpdates,
            };
        }

        internal Alias Alias;
        internal Site Site;
        internal Page Page;
        internal Module Module;
        internal IBlock Block;

        #endregion

    }
}
