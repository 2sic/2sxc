using Oqtane.Models;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Installation;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Render.Sys.RenderBlock;
using ToSic.Sxc.Web.Sys.LightSpeed;
using ToSic.Sxc.Web.Sys.Url;
using Page = Oqtane.Models.Page;

namespace ToSic.Sxc.Oqt.Server.Blocks;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class OqtSxcViewBuilder : ServiceBase, IOqtSxcViewBuilder
{
    #region Constructor and DI

    public OqtSxcViewBuilder(
        Output.OqtPageOutput pageOutput,
        IContextOfBlock contextOfBlockEmpty,
        BlockOfModule blockModuleEmpty,
        ISxcCurrentContextService currentContextServiceForLookUps,
        ILogStore logStore,
        GlobalTypesCheck globalTypesCheck,
        IOutputCache outputCache,
        Generator<IBlockBuilder> blockBuilderGenerator) : base($"{OqtConstants.OqtLogPrefix}.Buildr", connect: [pageOutput, contextOfBlockEmpty, blockModuleEmpty, currentContextServiceForLookUps, globalTypesCheck, outputCache, pageOutput, blockBuilderGenerator])
    {
        _contextOfBlockEmpty = contextOfBlockEmpty;
        _blockModuleEmpty = blockModuleEmpty;
        _currentContextServiceForLookUps = currentContextServiceForLookUps;
        _globalTypesCheck = globalTypesCheck;
        OutputCache = outputCache;
        PageOutput = pageOutput;
        _blockBuilderGenerator = blockBuilderGenerator;
        logStore.Add("oqt-view", Log);
    }

    public Output.OqtPageOutput PageOutput { get; }
    private readonly IContextOfBlock _contextOfBlockEmpty;
    private readonly BlockOfModule _blockModuleEmpty;
    private readonly ISxcCurrentContextService _currentContextServiceForLookUps;
    private readonly GlobalTypesCheck _globalTypesCheck;
    private readonly Generator<IBlockBuilder> _blockBuilderGenerator;

    #endregion

    #region Prepare

    /// <summary>
    /// Render must always be the first thing to be called - to ensure that afterward both headers and html are known.
    /// </summary>
    public OqtViewResultsDto Render(Alias alias, Site site, Page page, Module module, bool preRender)
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
        LogTimer.DoInTimer(() => Log.Do(timer: true, action: () =>
        {
            #region Lightspeed output caching
            var useLightspeed = OutputCache?.IsEnabled ?? false;
            if (OutputCache?.Existing != null) Log.A("Lightspeed hit - will use cached");
            var renderResult = OutputCache?.Existing?.Data
                               ?? _blockBuilderGenerator.New().Setup(Block).Run(true, specs: new()
                               {
                                   UseLightspeed = useLightspeed,
                                   IncludeAllAssetsInOqtane = site.RenderMode == "Interactive",
                               });
            finalMessage = !useLightspeed ? "" :
                OutputCache?.Existing?.Data != null ? "⚡⚡" : "⚡⏳";
            OutputCache?.Save(renderResult);

            #endregion

            PageOutput.Init(this, renderResult);

            ret = new()
            {
                Html = renderResult.Html,
                TemplateResources = PageOutput.GetSxcResources(),
                SxcContextMetaName = PageOutput.AddContextMeta
                    ? PageOutput.ContextMetaName
                    : null,
                SxcContextMetaContents = PageOutput.AddContextMeta
                    ? PageOutput.ContextMetaContents()
                    : null,
                SxcScripts = PageOutput.Scripts().ToList(),
                SxcStyles = PageOutput.Styles().ToList(),
                PageProperties = PageOutput.GetOqtPagePropertyChangesList(renderResult.PageChanges),
                HeadChanges = PageOutput.GetHeadChanges(),
                HttpHeaders = ConvertHttpHeaders(renderResult.HttpHeaders),
                // CSP settings
                CspEnabled = renderResult.CspEnabled,
                CspEnforced = renderResult.CspEnforced,
                CspParameters = renderResult.CspParameters
                    .Select(c => c.NvcToString())
                    .ToList(), // convert NameValueCollection to (query) string because can't serialize NameValueCollection to json
            };
        }));
        LogTimer.Done(OutputCache?.Existing?.Data?.IsError ?? false ? "⚠️" : finalMessage);

        // Check if there is less than 50 global types and warn user to restart application
        // HACK: in v14.03 this check was moved bellow LogTimer.DoInTimer because we got exception (probably timing issue)
        // "Object reference not set to an instance of an object. at ToSic.Eav.Apps.AppStates.Get(IAppIdentity app)"
        // TODO: STV find correct fix
        if (_globalTypesCheck.WarnIfGlobalTypesAreNotLoaded(out var oqtViewResultsDtoWarning2))
            return oqtViewResultsDtoWarning2;

        return ret;
    }

    // convert System.Collections.Generic.IList<ToSic.Sxc.Web.PageService.HttpHeader> to System.Collections.Generic.IList<ToSic.Sxc.Oqt.Shared.HttpHeader>
    private static IList<HttpHeader> ConvertHttpHeaders(IList<Sys.Render.PageContext.HttpHeader> httpHeaders) 
        => httpHeaders.Select(httpHeader => new HttpHeader(httpHeader.Name, httpHeader.Value)).ToList();

    internal Alias Alias;
    internal Site Site;
    public Page Page { get; private set; }
    internal Module Module;
    internal bool PreRender;

    private IBlock Block => _blockGetOnce.Get(() => LogTimer.DoInTimer(() =>
    {
        var ctx = _contextOfBlockEmpty.Init(Page.PageId, Module);
        var block = _blockModuleEmpty.GetBlockOfModule(ctx);

        // Special for Oqtane - normally the IContextResolver is only used in WebAPIs
        // But the ModuleLookUp and PageLookUp also rely on this, so the IContextResolver must know about this for now
        // In future, we should find a better way for this, so that IContextResolver is really only used on WebApis
        _currentContextServiceForLookUps.AttachBlock(block);
        return block;
    }));
    private readonly GetOnce<IBlock> _blockGetOnce = new();

    private ILogCall LogTimer => _logTimer.Get(() => Log.Fn(message: $"PreRender:{PreRender}, Page:{Page?.PageId} '{Page?.Name}', Module:{Module?.ModuleId} '{Module?.Title}'"));
    private readonly GetOnce<ILogCall> _logTimer = new();


    private IOutputCache OutputCache => _oc.Get(() => field.Init(Module.ModuleId, Page?.PageId ?? 0, Block));
    private readonly GetOnce<IOutputCache> _oc = new();

    #endregion
}