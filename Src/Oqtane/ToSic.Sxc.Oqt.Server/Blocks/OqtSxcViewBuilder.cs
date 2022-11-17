using Oqtane.Models;
using System.Linq;
using ToSic.Eav.Documentation;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Installation;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Web.LightSpeed;
using ToSic.Sxc.Web.Url;
using Page = Oqtane.Models.Page;

namespace ToSic.Sxc.Oqt.Server.Blocks
{
    [PrivateApi]
    public class OqtSxcViewBuilder : HasLog
    {

        #region Constructor and DI

        public OqtSxcViewBuilder(
            Output.OqtPageOutput pageOutput,
            IContextOfBlock contextOfBlockEmpty,
            BlockFromModule blockModuleEmpty,
            IContextResolver contextResolverForLookUps,
            LogHistory logHistory,
            GlobalTypesCheck globalTypesCheck,
            IOutputCache outputCache
        ) : base($"{OqtConstants.OqtLogPrefix}.Buildr")
        {
            _contextOfBlockEmpty = contextOfBlockEmpty;
            _blockModuleEmpty = blockModuleEmpty;
            _contextResolverForLookUps = contextResolverForLookUps;
            _globalTypesCheck = globalTypesCheck;
            _outputCache = outputCache;
            PageOutput = pageOutput;
            logHistory.Add("oqt-view", Log);
        }

        public Output.OqtPageOutput PageOutput { get; }
        private readonly IContextOfBlock _contextOfBlockEmpty;
        private readonly BlockFromModule _blockModuleEmpty;
        private readonly IContextResolver _contextResolverForLookUps;
        private readonly GlobalTypesCheck _globalTypesCheck;
        private readonly IOutputCache _outputCache;

        #endregion

        #region Prepare

        /// <summary>
        /// Prepare must always be the first thing to be called - to ensure that afterwards both headers and html are known.
        /// </summary>
        public OqtViewResultsDto Prepare(Alias alias, Site site, Page page, Module module, bool preRender)
        {
            Alias = alias;
            Site = site;
            Page = page;
            Module = module;
            PreRender = preRender;

            // Check for installation errors before even trying to build a view, and otherwise return this object if Refs are missing.
            if (RefsInstalledCheck.WarnIfRefsAreNotInstalled(out var oqtViewResultsDtoWarning)) return oqtViewResultsDtoWarning;

            OqtViewResultsDto ret = null;
            var finalMessage = "";
            LogTimer.DoInTimer(() =>
            {
                #region Lightspeed output caching
                var callLog = Log.Fn(startTimer: true);
                if (OutputCache?.Existing != null) Log.A("Lightspeed hit - will use cached");
                var renderResult = OutputCache?.Existing?.Data ?? Block.BlockBuilder.Run(true);
                finalMessage = OutputCache?.IsEnabled != true ? "" : OutputCache?.Existing?.Data != null ? "⚡⚡" : "⚡⏳";
                OutputCache?.Save(renderResult);
                #endregion
                PageOutput.Init(this, renderResult);

                ret = new OqtViewResultsDto
                {
                    Html = renderResult.Html,
                    TemplateResources = PageOutput.GetSxcResources(),
                    SxcContextMetaName = PageOutput.AddContextMeta ? PageOutput.ContextMetaName : null,
                    SxcContextMetaContents = PageOutput.AddContextMeta ? PageOutput.ContextMetaContents() : null,
                    SxcScripts = PageOutput.Scripts().ToList(),
                    SxcStyles = PageOutput.Styles().ToList(),
                    PageProperties = PageOutput.GetOqtPagePropertyChangesList(renderResult.PageChanges),
                    HttpHeaders = renderResult.HttpHeaders,
                    CspEnabled = renderResult.CspEnabled,
                    CspEnforced = renderResult.CspEnforced,
                    CspParameters = renderResult.CspParameters.Select(c => c.NvcToString()).ToList(), // convert NameValueCollection to (query) string because can't serialize NameValueCollection to json
                };
                callLog.Done();
            });
            LogTimer.Done(OutputCache?.Existing?.Data?.IsError ?? false ? "⚠️" : finalMessage);

            // Check if there is less than 50 global types and warn user to restart application
            // HACK: in v14.03 this check was moved bellow LogTimer.DoInTimer because we got exception (probably timing issue)
            // "Object reference not set to an instance of an object. at ToSic.Eav.Apps.AppStates.Get(IAppIdentity app)"
            // TODO: STV find correct fix
            if (_globalTypesCheck.WarnIfGlobalTypesAreNotLoaded(out var oqtViewResultsDtoWarning2)) return oqtViewResultsDtoWarning2;

            return ret;
        }

        internal Alias Alias;
        internal Site Site;
        internal Page Page;
        internal Module Module;
        internal bool PreRender;

        internal IBlock Block => _blockGetOnce.Get(() => LogTimer.DoInTimer(() =>
        {
            var ctx = _contextOfBlockEmpty.Init(Page.PageId, Module, Log);
            var block = _blockModuleEmpty.Init(ctx, Log);
            // Special for Oqtane - normally the IContextResolver is only used in WebAPIs
            // But the ModuleLookUp and PageLookUp also rely on this, so the IContextResolver must know about this for now
            // In future, we should find a better way for this, so that IContextResolver is really only used on WebApis
            _contextResolverForLookUps.AttachRealBlock(() => block);
            return block;
        }));
        private readonly GetOnce<IBlock> _blockGetOnce = new();

        protected LogCall LogTimer => _logTimer.Get(() => Log.Fn(message: $"PreRender:{PreRender}, Page:{Page?.PageId} '{Page?.Name}', Module:{Module?.ModuleId} '{Module?.Title}'"));
        private readonly GetOnce<LogCall> _logTimer = new();


        protected IOutputCache OutputCache => _oc.Get(() => _outputCache.Init(Log).Init(Module.ModuleId, Page?.PageId ?? 0, Block));
        private readonly GetOnce<IOutputCache> _oc = new();

        #endregion
    }
}
