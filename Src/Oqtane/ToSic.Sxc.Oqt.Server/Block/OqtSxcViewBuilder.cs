using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Oqtane.Models;
using Oqtane.Shared;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Razor.Engine.DbgWip;

namespace ToSic.Sxc.Oqt.Server.Block
{
    [PrivateApi]
    public class OqtSxcViewBuilder : HasLog, ISxcOqtane
    {
        #region Constructor and DI

        public OqtSxcViewBuilder(OqtAssetsAndHeaders assetsAndHeaders, RazorReferenceManager debugRefMan, OqtState oqtState
            ) : base($"{OqtConstants.OqtLogPrefix}.Buildr")
        {

            _assetsAndHeaders = assetsAndHeaders;
            _debugRefMan = debugRefMan;
            _oqtState = oqtState.Init(Log);
            // add log to history!
            History.Add("oqt-view", Log);
        }

        private IOqtAssetsAndHeader AssetsAndHeaders => _assetsAndHeaders;
        private readonly OqtAssetsAndHeaders _assetsAndHeaders;
        private readonly RazorReferenceManager _debugRefMan;
        private readonly OqtState _oqtState;

        #endregion

        #region Prepare

        /// <summary>
        /// Prepare must always be the first thing to be called - to ensure that afterwards both headers and html are known.
        /// </summary>
        public OqtViewResultsDto Prepare(Alias alias, Site site, Oqtane.Models.Page page, Module module)
        {
            //if (_renderDone) throw new Exception("already prepared this module");
            
            // Check for this error before even trying to build a view, and otherwise return this object if Refs are missing.
            if (CheckForRefs(out var oqtViewResultsDto)) return oqtViewResultsDto;

            Alias = alias;
            Site = site;
            Page = page;
            Module = module;

            Block = _oqtState.GetBlockOfModule(page.PageId, module);

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

        private static bool CheckForRefs(out OqtViewResultsDto oqtViewResultsDto)
        {
            var errorMessage = string.Empty;
            
            // Check for "refs" folder.
            // https://github.com/oqtane/oqtane.framework/issues/1272
            var dllLocation = AppContext.BaseDirectory;
            var refsPath = Path.Combine(dllLocation, "refs");
            if (!Directory.Exists(refsPath)) errorMessage  = "<strong>Warning:</strong> The \"refs\" folder is missing. Please ensure that <strong>Razor.Compiler.Dependencies.zip</strong> is unzipped as explained in the installation recipe <a href=\"https://azing.org/2sxc/r/fOG3aByY\" target=\"new\">https://azing.org/2sxc/r/fOG3aByY</a>.";

            oqtViewResultsDto = new OqtViewResultsDto
            {
                ErrorMessage = errorMessage,
            };

            return !string.IsNullOrEmpty(errorMessage);
        }
    }
}
