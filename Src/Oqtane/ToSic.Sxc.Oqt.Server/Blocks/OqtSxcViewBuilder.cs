using Oqtane.Models;
using System.Linq;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Installation;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Models;
using Page = Oqtane.Models.Page;

namespace ToSic.Sxc.Oqt.Server.Blocks
{
    [PrivateApi]
    public class OqtSxcViewBuilder : HasLog
    {

        #region Constructor and DI

        public OqtSxcViewBuilder(
            Blocks.Output.OqtPageOutput pageOutput, 
            IContextOfBlock contextOfBlockEmpty, 
            BlockFromModule blockModuleEmpty,
            IContextResolver contextResolverForLookUps,
            LogHistory logHistory,
            GlobalTypesCheck globalTypesCheck
        ) : base($"{OqtConstants.OqtLogPrefix}.Buildr")
        {
            _contextOfBlockEmpty = contextOfBlockEmpty;
            _blockModuleEmpty = blockModuleEmpty;
            _contextResolverForLookUps = contextResolverForLookUps;
            _globalTypesCheck = globalTypesCheck;
            PageOutput = pageOutput;
            logHistory.Add("oqt-view", Log);
        }

        public Blocks.Output.OqtPageOutput PageOutput { get; }
        private readonly IContextOfBlock _contextOfBlockEmpty;
        private readonly BlockFromModule _blockModuleEmpty;
        private readonly IContextResolver _contextResolverForLookUps;
        private readonly GlobalTypesCheck _globalTypesCheck;

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
            if (RefsInstalledCheck.WarnIfRefsAreNotInstalled(out var oqtViewResultsDtoWarning)) return oqtViewResultsDtoWarning;

            // Check if there is less than 50 global types and warn user to restart application
            if (_globalTypesCheck.WarnIfGlobalTypesAreNotLoaded(out var oqtViewResultsDtoWarning2)) return oqtViewResultsDtoWarning2;

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

            // #Lightspeed
            var renderResult = Block.BlockBuilder.Run(true);

            PageOutput.Init(this, renderResult);

            var ret = new OqtViewResultsDto
            {
                Html = renderResult.Html, 
                TemplateResources = PageOutput.GetSxcResources(),
                SxcContextMetaName = PageOutput.AddContextMeta ? PageOutput.ContextMetaName : null,
                SxcContextMetaContents = PageOutput.AddContextMeta ? PageOutput.ContextMetaContents(): null,
                SxcScripts = PageOutput.Scripts().ToList(),
                SxcStyles = PageOutput.Styles().ToList(),
                PageProperties = PageOutput.GetOqtPagePropertyChangesList(renderResult.PageChanges)
            };

            return ret;
        }



        internal Alias Alias;
        internal Site Site;
        internal Page Page;
        internal Module Module;
        internal IBlock Block
        {
            get
            {
                if (_blockLoaded) return _block;
                _blockLoaded = true;
                var ctx = _contextOfBlockEmpty.Init(Page.PageId, Module, Log);
                _block = _blockModuleEmpty.Init(ctx, Log);
                
                // Special for Oqtane - normally the IContextResolver is only used in WebAPIs
                // But the ModuleLookUp and PageLookUp also rely on this, so the IContextResolver must know about this for now
                // In future, we should find a better way for this, so that IContextResolver is really only used on WebApis
                _contextResolverForLookUps.AttachRealBlock(() => _block);
                return _block;
            }
        }

        private IBlock _block;
        private bool _blockLoaded;

        #endregion
    }
}
