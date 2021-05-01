using Microsoft.AspNetCore.Http;
using Oqtane.Models;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Oqt.Server.Page;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Oqt.Shared.Run;
using ToSic.Sxc.Razor.Engine.DbgWip;

namespace ToSic.Sxc.Oqt.Server
{
    public class SxcOqtane : HasLog, ISxcOqtane
    {
        public SiteState SiteState { get; }

        #region Constructor and DI

        public SxcOqtane(OqtAssetsAndHeaders assetsAndHeaders, RazorReferenceManager debugRefMan, OqtTempInstanceContext oqtTempInstanceContext,
            IServiceProvider serviceProvider, Lazy<SiteStateInitializer> siteStateInitializerLazy, IHttpContextAccessor httpContextAccessor, SiteState siteState, Lazy<OqtState> oqtState
            ) : base($"{OqtConstants.OqtLogPrefix}.Buildr")
        {
            SiteState = siteState;
            _assetsAndHeaders = assetsAndHeaders;
            _debugRefMan = debugRefMan;
            _oqtTempInstanceContext = oqtTempInstanceContext;
            _serviceProvider = serviceProvider;
            _siteStateInitializerLazy = siteStateInitializerLazy;
            _httpContextAccessor = httpContextAccessor;
            _oqtState = oqtState;
            // add log to history!
            History.Add("oqt-view", Log);
        }

        private IOqtAssetsAndHeader AssetsAndHeaders => _assetsAndHeaders;
        private readonly OqtAssetsAndHeaders _assetsAndHeaders;
        private readonly RazorReferenceManager _debugRefMan;
        private readonly OqtTempInstanceContext _oqtTempInstanceContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly Lazy<SiteStateInitializer> _siteStateInitializerLazy;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Lazy<OqtState> _oqtState;

        #endregion

        #region Prepare

        /// <summary>
        /// Prepare must always be the first thing to be called - to ensure that afterwards both headers and html are known.
        /// </summary>
        public OqtViewResultsDto Prepare(Alias alias, Site site, Oqtane.Models.Page page, Module module)
        {
            //if (_renderDone) throw new Exception("already prepared this module");

            // HACKS: STV POC - indirectly share information
            if (alias != null) _httpContextAccessor?.HttpContext?.Items.TryAdd("AliasFor2sxc", alias);
            if (module != null) _httpContextAccessor?.HttpContext?.Items.TryAdd("ModuleForLookUp", module);

            Alias = alias;
            Site = site;
            Page = page;
            Module = module;

            Block = _oqtState.Value.GetBlock(page.PageId, module, Log);

            _assetsAndHeaders.Init(this);
            var generatedHtml = Block.BlockBuilder.Render() ;
            Resources = Block.BlockBuilder.Assets.Select(a => new SxcResource
            {
                ResourceType = a.IsJs ? ResourceType.Script : ResourceType.Stylesheet,
                Url = a.Url,
                IsExternal = a.IsExternal,
                Content = a.Content,
                UniqueId = a.Id
            }).ToList();

            //_renderDone = true;

            return new OqtViewResultsDto
            {
                Html = generatedHtml,
                TemplateResources = Resources,
                //AddContextMeta = AssetsAndHeaders.AddContextMeta,
                SxcContextMetaName = AssetsAndHeaders.AddContextMeta ? AssetsAndHeaders.ContextMetaName : null,
                SxcContextMetaContents = AssetsAndHeaders.AddContextMeta ? AssetsAndHeaders.ContextMetaContents(): null,
                SxcScripts = AssetsAndHeaders.Scripts().ToList(),
                SxcStyles = AssetsAndHeaders.Styles().ToList(),
            };
        }

        internal Alias Alias;
        internal Site Site;
        internal Oqtane.Models.Page Page;
        internal Module Module;
        internal IBlock Block;

        //private bool _renderDone;
        //private string GeneratedHtml { get; set; }

        private List<SxcResource> Resources { get; set; }

        #endregion

        public string Test() => _debugRefMan.CompilationReferences.Count.ToString();

    }
}
