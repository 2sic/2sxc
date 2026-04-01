using DotNetNuke.Entities.Modules;
using ToSic.Eav.Web.Sys;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.BlockBuilder;
using ToSic.Sxc.Dnn.Features;
using ToSic.Sxc.Dnn.Install;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Render.Sys.RenderBlock;
using ToSic.Sxc.Web.Sys.LightSpeed;

namespace ToSic.Sxc.Dnn;

internal sealed class ModuleViewRuntime(
    ModuleInfo moduleConfiguration,
    int moduleId,
    int tabId,
    IServiceProvider serviceProvider,
    Func<string> getOptionalDetailedLogToAttach)
{
    public IServiceProvider ServiceProvider => serviceProvider;

    public TService GetService<TService>(ILog parentLog = null)
        => ServiceProvider.Build<TService>(parentLog);

    public IBlock Block => _blockGetOnce.Get(Log, () => GetService<IModuleAndBlockBuilder>(Log).BuildBlock(moduleConfiguration, null), timer: true);
    private readonly GetOnce<IBlock> _blockGetOnce = new();

    private IBlockBuilder BlockBuilder => field ??= GetService<IBlockBuilder>(Log).Setup(Block);

    public HttpRequestLoggingScoped RequestLogging => field
        ??= ServiceProvider.Build<Generator<HttpRequestLoggingScoped, HttpRequestLoggingScoped.Opts>>()
            .New(new() { Segment = "module", RootName = "Sxc.View" });

    public ILog Log => field ??= new Log("Sxc.View", RequestLogging.RootLog);

    public IOutputCache OutputCache => field ??= GetService<IOutputCache>(Log).Init(moduleId, tabId, Block);

    public void BeginRequest(string moduleTitle)
        => RequestLogging.RootLog.A($"Module Title New: '{moduleTitle}'");

    public void Prepare(PortalModuleBase portalModule = null)
    {
        var block = Block;
        var checkPortalIsReady = OutputCache.Existing == null;
        if (!checkPortalIsReady)
            return;

        if (portalModule != null)
        {
            if (!DnnReadyCheckTurbo.QuickCheckSiteAndAppFoldersAreReady(portalModule, Log))
                GetService<DnnReadyCheckTurbo>(Log).EnsureSiteAndAppFoldersAreReady(portalModule, block);
            return;
        }

        if (!DnnReadyCheckTurbo.QuickCheckSiteAndAppFoldersAreReady(moduleId, tabId, Log))
            GetService<DnnReadyCheckTurbo>(Log).EnsureSiteAndAppFoldersAreReady(moduleConfiguration, moduleId, tabId, block);
    }

    public ModuleViewRenderState Render(bool renderNaked)
    {
        var cachedResult = OutputCache.Existing?.Data;
        var cacheHit = cachedResult != null;
        var useLightspeed = OutputCache.IsEnabled;
        var finalMessage = !useLightspeed ? "" : cacheHit ? "⚡⚡" : "⚡⏳";

        var renderResult = cachedResult ?? RenderViewAndGatherJsCssSpecs(renderNaked, useLightspeed);

        RequestLogging.StoreEntry.TryUpdateSpecs(() => new SpecsForLogHistory().BuildSpecsForLogHistory(Block));

        if (!cacheHit)
            OutputCache.Save(renderResult);

        return new(renderResult, cacheHit, finalMessage);
    }

    public string BuildFriendlyErrorHtml(Exception ex, bool wrapInContext = true)
    {
        var renderingHelper = GetService<IRenderingHelper>(Log).Init(Block);
        var msg = renderingHelper.DesignErrorMessage([ex], true,
            additionalInfo: $" - ℹ️ CONTEXT: Page: {tabId}; Module: {moduleId}");

        if (!wrapInContext)
            return msg;

        try
        {
            if (Block.Context.Permissions.IsContentAdmin)
                msg = renderingHelper.WrapInContext(msg,
                    instanceId: Block.ParentId,
                    contentBlockId: Block.ContentBlockId,
                    editContext: true,
                    errorCode: BlockBuildingConstants.ErrorGeneral,
                    exsOrNull: [ex]);
        }
        catch
        {
            /* ignore */
        }

        return msg;
    }

    private IRenderResult RenderViewAndGatherJsCssSpecs(bool renderNaked, bool useLightspeed)
    {
        var l = Log.Fn<IRenderResult>(message: $"module {moduleId} on page {tabId}", timer: true);

        if (renderNaked)
            BlockBuilder.WrapInDiv = false;

        var result = (RenderResult)BlockBuilder.Run(
            true,
            specs: new()
            {
                UseLightspeed = useLightspeed,
                RenderEngineResult = GetService<DnnRequirements>(Log).GetMessageForRequirements()
            }
        );

        if (result.Errors?.Any() ?? false)
        {
            var warnings = result.Errors
                .Select(e => BlockBuilder.RenderingHelper.DesignError(e));

            result = result with { Html = string.Join("", warnings) + result.Html };
        }

        result = result with { Html = result.Html + getOptionalDetailedLogToAttach() };
        return l.ReturnAsOk(result);
    }
}

internal sealed record ModuleViewRenderState(IRenderResult RenderResult, bool CacheHit, string FinalMessage);
