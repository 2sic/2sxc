using System.Collections.Generic;
using System.Linq;
using Oqtane.Models;
using Oqtane.Shared;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Beta.LightSpeed;
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

        public OqtSxcViewBuilder(OqtAssetsAndHeaders assetsAndHeaders, OqtState oqtState/*, IClientDependencyOptimizer oqtClientDependencyOptimizer*/) : base($"{OqtConstants.OqtLogPrefix}.Buildr")
        {
            AssetsAndHeaders = assetsAndHeaders;
            _oqtState = oqtState.Init(Log);
            //OqtClientDependencyOptimizer = oqtClientDependencyOptimizer.Init(Log);
            // add log to history!
            History.Add("oqt-view", Log);
        }

        public OqtAssetsAndHeaders AssetsAndHeaders { get; }
        private readonly OqtState _oqtState;

        #endregion

        #region Prepare

        /// <summary>
        /// Prepare must always be the first thing to be called - to ensure that afterwards both headers and html are known.
        /// </summary>
        public OqtViewResultsDto Prepare(Alias alias, Site site, Page page, Module module)
        {
            Alias = alias;
            Site = site;
            Page = page;
            Module = module;

            // Check for installation errors before even trying to build a view, and otherwise return this object if Refs are missing.
            if (RefsInstalledCheck.WarnIfRefsAreNotInstalled(out var oqtViewResultsDto)) return oqtViewResultsDto;

            #region Lightspeed - very experimental - deactivate before distribution
            //if (Lightspeed.HasCache(module.ModuleId))
            //{
            //    Log.Add("Lightspeed enabled, has cache");
            //    PreviousCache = Lightspeed.Get(module.ModuleId);
            //}

            //if (PreviousCache == null)
            //{
            //    NewCache = new OutputCacheItem();
            //}
            //else
            //{
            //    Log.Add("Lightspeed hit - will use cached");
            //}
            #endregion

            AssetsAndHeaders.Init(this);

            // #Lightspeed
            var renderResult = PreviousCache?.Data ?? Block.BlockBuilder.Run();

            var resources = renderResult.Assets.Select(a => new SxcResource
            {
                ResourceType = a.IsJs ? ResourceType.Script : ResourceType.Stylesheet,
                Url = a.Url,
                IsExternal = a.IsExternal,
                Content = a.Content,
                UniqueId = a.Id
            }).ToList();

            //// #Lightspeed
            //if (NewCache != null)
            //{
            //    Log.Add("Adding to lightspeed");
            //    NewCache.Data = renderResult;
            //    Lightspeed.Add(Module.ModuleId, NewCache);
            //}

            var ret = new OqtViewResultsDto
            {
                Html = renderResult.Html, 
                TemplateResources = resources,
                SxcContextMetaName = AssetsAndHeaders.AddContextMeta ? AssetsAndHeaders.ContextMetaName : null,
                SxcContextMetaContents = AssetsAndHeaders.AddContextMeta ? AssetsAndHeaders.ContextMetaContents(): null,
                SxcScripts = AssetsAndHeaders.Scripts().ToList(),
                SxcStyles = AssetsAndHeaders.Styles().ToList(),
                PageProperties = AssetsAndHeaders.GetOqtPagePropertyChangesList(renderResult.PageChanges)
            };

            return ret;
        }

        internal Alias Alias;
        internal Site Site;
        internal Page Page;
        internal Module Module;
        internal IBlock Block => _block ??= _oqtState.GetBlockOfModule(Page.PageId, Module);
        private IBlock _block;

        #endregion

        private OutputCacheManager Lightspeed => _lightspeed ??= new OutputCacheManager();
        private OutputCacheManager _lightspeed;
        private OutputCacheItem PreviousCache;
        private OutputCacheItem NewCache;

    }
}
