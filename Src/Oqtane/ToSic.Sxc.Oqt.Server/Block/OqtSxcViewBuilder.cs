using System.Collections.Generic;
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
        public IClientDependencyOptimizer OqtClientDependencyOptimizer { get; }

        #region Constructor and DI

        public OqtSxcViewBuilder(OqtAssetsAndHeaders assetsAndHeaders, OqtState oqtState, IClientDependencyOptimizer oqtClientDependencyOptimizer) : base($"{OqtConstants.OqtLogPrefix}.Buildr")
        {
            _assetsAndHeaders = assetsAndHeaders;
            _oqtState = oqtState.Init(Log);
            OqtClientDependencyOptimizer = oqtClientDependencyOptimizer.Init(Log);
            // add log to history!
            History.Add("oqt-view", Log);
        }

        private OqtAssetsAndHeaders AssetsAndHeaders => _assetsAndHeaders;
        private readonly OqtAssetsAndHeaders _assetsAndHeaders;

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
            var generatedHtml = Block.BlockBuilder.Render(); // TODO: RUN INSTEAD OF Render

            var pageHeadAssets = PageHeadAssets();
            if (pageHeadAssets.Count > 0)
                Block.BlockBuilder.Assets.AddRange(pageHeadAssets);

            var resources = Block.BlockBuilder.Assets.Select(a => new SxcResource
            {
                ResourceType = a.IsJs ? ResourceType.Script : ResourceType.Stylesheet,
                Url = a.Url,
                IsExternal = a.IsExternal,
                Content = a.Content,
                UniqueId = a.Id
            }).ToList();

            return new OqtViewResultsDto
            {
                Html = generatedHtml,
                TemplateResources = resources,
                SxcContextMetaName = AssetsAndHeaders.AddContextMeta ? AssetsAndHeaders.ContextMetaName : null,
                SxcContextMetaContents = AssetsAndHeaders.AddContextMeta ? AssetsAndHeaders.ContextMetaContents(): null,
                SxcScripts = AssetsAndHeaders.Scripts().ToList(),
                SxcStyles = AssetsAndHeaders.Styles().ToList(),
                PageProperties = AssetsAndHeaders.GetPagePropertyChanges(),
            };
        }

        private List<ClientAssetInfo> PageHeadAssets()
        {
            AssetsAndHeaders.GetPageHeadUpdates().ToList()
                .ForEach(html => OqtClientDependencyOptimizer.Process(html));
            return OqtClientDependencyOptimizer.Assets;
        }

        internal Alias Alias;
        internal Site Site;
        internal Page Page;
        internal Module Module;
        internal IBlock Block;

        #endregion

    }
}
